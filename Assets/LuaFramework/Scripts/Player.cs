using UnityEngine;
using System;

namespace Game
{
    public class Player
    {
        public string name;
        public int hp;

        public static int playerCount = 0;

        public event Action<int> OnHurt;

        public Player(string name, int hp)
        {
            this.name = name;
            this.hp = hp;
            playerCount++;
        }

        public void TakeDamage(int dmg)
        {
            hp -= dmg;
            OnHurt?.Invoke(hp);
        }

        public static void PrintPlayerCount()
        {
            Debug.Log("Player Count = " + playerCount);
        }
    }
}
