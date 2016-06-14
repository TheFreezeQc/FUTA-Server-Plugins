﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace INF3
{
    public static class Effects
    {
        public static void SelfExploed(this Entity player)
        {
            Function.SetEntRef(-1);
            Function.Call("playfx", Function.Call<int>("loadfx", "props/barrelexp"), player.Call<Vector3>("gettagorigin", "j_head"));
            player.Call("playsound", "explo_mine");
            player.Call("finishplayerdamage", player, player, player.GetField<int>("maxhealth"), 0, 0, "nuke_mp", player.Origin, "MOD_EXPLOSIVE", 0);
        }

        public static void RadiusExploed(this Entity player, Vector3 origin)
        {
            Function.SetEntRef(-1);
            Function.Call("RadiusDamage", origin, 500, 500, 500, player, "MOD_EXPLOSIVE", "nuke_mp");
            Function.Call("playfx", Function.Call<int>("loadfx", "explosions/tanker_explosion"), origin);
            player.Call("playsound", "cobra_helicopter_crash");
        }
    }
}
