using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Inventory.Data.Services;
using Inventory.Web.Logic;
using System.Web.Http;
using System.Web.Mvc;

namespace Inventory.Web
{
    public class ContainerConfig
    {
        internal static void RegisterContainer(HttpConfiguration httpconfiguration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly); //MVC
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly); //API
            builder.RegisterType<CommonLogic>().As<ICommonLogic>().InstancePerRequest();
            builder.RegisterType<InvoiceLogic>().As<IInvoiceLogic>().InstancePerRequest();
            builder.RegisterType<InvoiceItemLogic>().As<IInvoiceItemLogic>().InstancePerRequest();
            builder.RegisterType<AssetLogic>().As<IAssetLogic>().InstancePerRequest();
            builder.RegisterType<LocationLogic>().As<ILocationLogic>().InstancePerRequest();
            builder.RegisterType<BuildingLogic>().As<IBuildingLogic>().InstancePerRequest();
            builder.RegisterType<RoomLogic>().As<IRoomLogic>().InstancePerRequest();
            builder.RegisterType<UserLogic>().As<IUserLogic>().InstancePerRequest();
            builder.RegisterType<ManuLogic>().As<IManuLogic>().InstancePerRequest();
            builder.RegisterType<ProductLogic>().As<IProductLogic>().InstancePerRequest();
            builder.RegisterType<SupplierLogic>().As<ISupplierLogic>().InstancePerRequest();
            builder.RegisterType<UserAssetLogic>().As<IUserAssetLogic>().InstancePerRequest();
            builder.RegisterType<AssetListLogic>().As<IAssetListLogic>().InstancePerRequest();
            builder.RegisterType<InventoryDbContext>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //MVC Framework
            httpconfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container); //WEB API
        }
    }
}