using Repository;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.maps;
using Tanks_Sever.tanks.loaders;
using Tanks_Sever.tanks.lobby.top;
using Tanks_Sever.tanks.main.procotol;
using Tanks_Sever.tanks.main.procotol.tcp;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.users.garage;
using Tanks_Sever.tanks.utils;

public class ProgramMain
{
    static void Main(string[] args)
    {
        ConfiguratorLoader.Init("Configs/configuration.cfg");
        initFactorys();
        GenericManager.Init("Configs/databaseConfig.cfg");
        TcpServer.Inject().Init();
        Console.ReadLine();
    }

    private static void initFactorys() 
    {
        GarageItemsLoader.LoadFromConfig("Configs/turrets.json", "Configs/hulls.json", "Configs/colormaps.json", "Configs/inventory.json", "Configs/effects.json");
        WeaponsFactory.Init("Configs/weapons/");
        HullsFactory.Init("Configs/hulls/");
        RankUtils.Init();
        MapsLoader.InitFactoryMaps();
    }
}
