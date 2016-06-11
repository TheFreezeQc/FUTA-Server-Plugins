﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace Exo
{
    public class Sharpshooter : BaseScript
    {
        private Weapon _currentWeapon;
        private Weapon _mulekickWeapon;
        private Random _rng = new Random();
        private HudElem _cycleTimer;
        private int _cycleRemaining = 30;

        public Sharpshooter()
        {
            Entity entity = Call<Entity>("getent", new Parameter[] { "care_package", "targetname" });
            Entity _airdropCollision = Call<Entity>("getent", new Parameter[] { entity.GetField<string>("target"), "targetname" });

            _currentWeapon = Weapon.GetRandomWeapon();
            _mulekickWeapon = Weapon.GetRandomWeapon();

            while (_mulekickWeapon.BaseName == _currentWeapon.BaseName)
            {
                _mulekickWeapon = Weapon.GetRandomWeapon();
            }

            OnInterval(100, () =>
            {
                if (Call<int>("getteamscore", "axis") == 1)
                {
                    SharpShooter_Tick();
                    return false;
                }
                return true;
            });

            PlayerConnected += player =>
            {
                OnPlayerSpawned(player);
                player.SpawnedPlayer += () => OnPlayerSpawned(player);
            };
        }

        private string FormatTime(int seconds)
        {
            return string.Format("{0}:{1}", seconds / 60, (seconds % 60).ToString().PadLeft(2, '0'));

        }

        public void SharpShooter_Tick()
        {
            _cycleTimer = HudElem.CreateServerFontString("objective", 1.4f);
            _cycleTimer.SetPoint("TOPLEFT", "TOPLEFT", 115, 5);
            _cycleTimer.HideWhenInMenu = true;

            OnInterval(1000, () =>
            {
                _cycleRemaining--;

                if (_cycleRemaining == 0)
                {
                    _cycleRemaining = _rng.Next(30, 61);

                    UpdateWeapon();
                }

                _cycleTimer.SetText("Weapon Cycling: " + FormatTime(_cycleRemaining));

                return true;
            });
        }

        public void UpdateWeapon()
        {
            _currentWeapon = Weapon.GetRandomWeapon();
            _mulekickWeapon = Weapon.GetRandomWeapon();

            while (_mulekickWeapon.BaseName == _currentWeapon.BaseName)
            {
                _mulekickWeapon = Weapon.GetRandomWeapon();
            }

            foreach (var player in Players)
            {
                if (player.GetTeam() == "allies")
                {
                    player.TakeAllWeapons();

                    player.GiveWeapon(_currentWeapon.Code);
                    player.Call("givemaxammo", _currentWeapon.Code);
                    if (player.GetField<int>("perk_mulekick") == 1)
                    {
                        player.GiveWeapon(_mulekickWeapon.Code);
                        player.Call("givemaxammo", _mulekickWeapon.Code);
                    }

                    player.AfterDelay(100, entity =>
                    {
                        entity.SwitchToWeaponImmediate(_currentWeapon.Code);
                        entity.Call("iprintlnbold", _currentWeapon.Name);
                    });

                    player.GamblerText("Weapon Cycled", new Vector3(1, 1, 1), new Vector3(0.3f, 0.3f, 0.9f), 1, 0.85f);
                }
            }
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (player.GetTeam() == "allies")
            {
                player.TakeAllWeapons();

                player.GiveWeapon(_currentWeapon.Code);
                player.Call("giveMaxAmmo", _currentWeapon.Code);

                player.AfterDelay(100, entity =>
                {
                    entity.SwitchToWeaponImmediate(_currentWeapon.Code);
                    entity.Call("iprintlnbold", _currentWeapon.Name);
                });

                player.OnInterval(1000, e =>
                {
                    if (player.IsAlive)
                    {
                        var weapon = player.CurrentWeapon;

                        if (weapon.StartsWith("rpg") || weapon.StartsWith("iw5_smaw") || weapon.StartsWith("m320") || weapon.StartsWith("uav") || weapon.StartsWith("stinger") || weapon.StartsWith("javelin") || weapon.StartsWith("gl"))
                        {
                            player.Call("giveMaxAmmo", weapon);
                        }

                        return true;
                    }
                    return false;
                });
            }
        }
    }
}