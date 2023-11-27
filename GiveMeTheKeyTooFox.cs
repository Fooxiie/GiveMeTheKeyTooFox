using Life;
using Life.Network;
using Life.VehicleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveMeTheKeyTooFox
{
    public class GiveMeTheKeyTooFox : Plugin
    {
        public GiveMeTheKeyTooFox(IGameAPI api) : base(api)
        {
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            SChatCommand sChatCommand = new SChatCommand("/giveCopro",
                "Donner la copropriété à la personne la plus proche",
                "/giveCopro", (Action<Player, string[]>)((player, arg) =>
                {
                    GiveCoPro(player);
                }));

            sChatCommand.Register();
        }

        private void GiveCoPro(Player player)
        {
            var closestPlayer = player.GetClosestPlayer();
            if (closestPlayer != null)
            {
                var closestVehicle = player.GetClosestVehicle();
                if (closestVehicle != null)
                {
                    LifeVehicle vehicle = Nova.v.GetVehicle(closestVehicle.vehicleDbId);

                    int ownerID = vehicle.permissions.owner.characterId;

                    if (ownerID == player.character.Id)
                    {
                        vehicle.AddCoOwner(new Life.PermissionSystem.Entity
                        {
                            characterId = closestPlayer.character.Id,
                        });

                        vehicle.SaveAndFake();

                        player.Notify("CoPropiétaire ajouté",
                            $"Vous avez donnez un double des cléfs à {closestPlayer.GetFullName()}",
                            NotificationManager.Type.Success);

                        closestPlayer.Notify("CoPropiétaire ajouté",
                            $"Vous avez reçus la copropriété du véhicule : {closestVehicle.plate}");

                        player.setup.TargetPlayClairon(60);

                        foreach (var aroundPlayer in Nova.closestPlayers)
                        {
                            aroundPlayer.CmdSendText(player.GetFullName() + " a donné un double des clefs à " + closestPlayer.GetFullName());
                        }
                        
                    } else
                    {
                        player.Notify("Erreur", "Le véhicule ne vous appartient pas.", NotificationManager.Type.Error);
                    }
                } else
                {
                    player.Notify("Erreur", "Aucun véhicule qui vous appartient dans les parages", NotificationManager.Type.Error);
                }
            } else
            {
                player.Notify("Erreur", "Personne n'est près de vous.", NotificationManager.Type.Error);
            }
        }
    }
}
