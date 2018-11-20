namespace DemoCognitiveServices.Helpers
{
    public static class Constantes
    {
        #region Constantes Face

        public readonly static string CuentaAdmin = "admin";
        public readonly static string FaceApiSubscriptionKey = "aqui-va-tu-subscription-key";
        public readonly static string FaceApiEndpoint = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/";
        public readonly static string Group = "democs"; //debe ser en minusculas y creado desde la app en primer lugar
        public readonly static string Detect = $"detect";
        public readonly static string Identify = $"identify";
        public readonly static string PersonGroup = $"persongroups/{Group}";
        public readonly static string Person = $"/persons";
        public readonly static string GroupTrain = $"/train";

        #endregion
    }
}
