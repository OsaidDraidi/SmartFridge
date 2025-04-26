namespace FridgeProject.Services
{
    public interface IGeminiService
    {
        Task<string> GetGeminiResponseAsync(string userInput);
    }
}
