//using Izenda.BI.Logic.CustomConfiguration;
//using Izenda.BI.Framework.Models.UserManagement;
//using Rhino.Licensing;

//namespace Mvc5StarterKit
//{
//    public static class IzendaConfig
//    {
//        public static void RegisterLoginLogic()
//        {
//            UserIntegrationConfig.ValidateToken = (ValidateTokenArgs args) =>
//            {
//                var token = args.AccessToken;
//                var user = IzendaBoundary.IzendaTokenAuthorization.GetUserInfo(token);
//                return new ValidateTokenResult { UserName = user.UserName, TenantUniqueName = user.TenantUniqueName };
//            };
//        }
//    }
//}