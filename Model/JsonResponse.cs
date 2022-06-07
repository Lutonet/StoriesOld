namespace Stories.Model;
public class JsonResponse
{
    public string JsonRequest { get; set; } = "";
    public bool IsError { get; set; } = false;

    public string ErrorMessage { get; set; } = "";

}
