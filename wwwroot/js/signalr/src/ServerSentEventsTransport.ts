// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

import { HttpClient } from "./HttpClient";
import { MessageHeaders } from "./IHubProtocol";
import { ILogger, LogLevel } from "./ILogger";
import { ITransport, TransferFormat } from "./ITransport";
import { EventSourceConstructor } from "./Polyfills";
import { Arg, getDataDetail, getUserAgentHeader, Platform, sendMessage } from "./Utils";

/** @private */
export class ServerSentEventsTransport implements ITransport {
    private readonly httpClient: HttpClient;
    private readonly accessTokenFactory: (() => string | Promise<string>) | undefined;
    private readonly logger: ILogger;
    private readonly logMessageContent: boolean;
    private readonly withCredentials: boolean;
    private readonly eventSourceConstructor: EventSourceConstructor;
    private eventSource?: EventSource;
    private url?: string;
    private headers: MessageHeaders;

    public onreceive: ((data: string | ArrayBuffer) => void) | null;
    public onclose: ((error?: Error) => void) | null;

    constructor(httpClient: HttpClient, accessTokenFactory: (() => string | Promise<string>) | undefined, logger: ILogger,
                logMessageContent: boolean, eventSourceConstructor: EventSourceConstructor, withCredentials: boolean, headers: MessageHeaders) {
        this.httpClient = httpClient;
        this.accessTokenFactory = accessTokenFactory;
        this.logger = logger;
        this.logMessageContent = logMessageContent;
        this.withCredentials = withCredentials;
        this.eventSourceConstructor = eventSourceConstructor;
        this.headers = headers;

        this.onreceive = null;
        this.onclose = null;
    }

    public async connect(url: string, transferFormat: TransferFormat): Promise<void> {
        Arg.isRequired(url, "url");
        Arg.isRequired(transferFormat, "transferFormat");
        Arg.isIn(transferFormat, TransferFormat, "transferFormat");

        this.logger.log(LogLevel.Trace, "(SSE transport) Connecting.");

        // set url before accessTokenFactory because this.url is only for send and we set the auth header instead of the query string for send
        this.url = url;

        if (this.accessTokenFactory) {
            const token = await this.accessTokenFactory();
            if (token) {
                url += (url.indexOf("?") < 0 ? "?" : "&") + `access_token=${encodeURIComponent(token)}`;
            }
        }

        return new Promise<void>((resolve, reject) => {
            let opened = false;
            if (transferFormat !== TransferFormat.Text) {
                reject(new Error("The Server-Sent Events transport only supports the 'Text' transfer format"));
                return;
            }

            let eventSource: EventSource;
            if (Platform.isBrowser || Platform.isWebWorker) {
                eventSource = new this.eventSourceConstructor(url, { withCredentials: this.withCredentials });
            } else {
                // Non-browser passes cookies via the dictionary
                const cookies = this.httpClient.getCookieString(url);
                const headers: MessageHeaders = {};
                headers.Cookie = cookies;
                const [name, value] = getUserAgentHeader();
                headers[name] = value;

                eventSource = new this.eventSourceConstructor(url, { withCredentials: this.withCredentials, headers: { ...headers, ...this.headers} } as EventSourceInit);
            }

            try {
                eventSource.onmessage = (e: MessageEvent) => {
                    if (this.onreceive) {
                        try {
                            this.logger.log(LogLevel.Trace, `(SSE transport) data received. ${getDataDetail(e.data, this.logMessageContent)}.`);
                            this.onreceive(e.data);
                        } catch (error) {
                            this.close(error);
                            return;
                        }
                    }
                };

                eventSource.onerror = (e: MessageEvent) => {
                    const error = new Error(e.data || "Error occurred");
                    if (opened) {
                        this.close(error);
                    } else {
                        reject(error);
                    }
                };

                eventSource.onopen = () => {
                    this.logger.log(LogLevel.Information, `SSE connected to ${this.url}`);
                    this.eventSource = eventSource;
                    opened = true;
                    resolve();
                };
            } catch (e) {
                reject(e);
                return;
            }
        });
    }

    public async send(data: any): Promise<void> {
        if (!this.eventSource) {
            return Promise.reject(new Error("Cannot send until the transport is connected"));
        }
        return sendMessage(this.logger, "SSE", this.httpClient, this.url!, this.accessTokenFactory, data, this.logMessageContent, this.withCredentials, this.headers);
    }

    public stop(): Promise<void> {
        this.close();
        return Promise.resolve();
    }

    private close(e?: Error) {
        if (this.eventSource) {
            this.eventSource.close();
            this.eventSource = undefined;

            if (this.onclose) {
                this.onclose(e);
            }
        }
    }
}