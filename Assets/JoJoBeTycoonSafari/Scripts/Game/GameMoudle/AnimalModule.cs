using Game.MessageCenter;
using Logger;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UFrame;
using UFrame.MessageCenter;
using UnityEngine;

namespace Game
{
    public class AnimalModule : GameModule
    {
        List<Animal> unLockAnimals;
        public AnimalModule(int orderID) : base(orderID) { }

        public override void Init()
        {
            //注册消息
            MessageManager.GetInstance().Regist((int)GameMessageDefine.GetUnlockAnimals, this.OnGetUnlockAnimals);

            unLockAnimals = new List<Animal>();

            //Run();
        }

        public override void Release()
        {
            MessageManager.GetInstance().UnRegist((int)GameMessageDefine.GetUnlockAnimals, this.OnGetUnlockAnimals);

            unLockAnimals.Clear();

            Stop();
        }

        public override void Tick(int deltaTimeMS)
        {
            if (!this.CouldRun())
            {
                return;
            }

        }

        protected void OnGetUnlockAnimals(Message msg)
        {
            unLockAnimals.Clear();
            for(int i = 0; i < 10; i++)
            {
                var animal = new Animal();
                animal.animalID = 10101 + i * 100;
                animal.animalLevel = 1;
                unLockAnimals.Add(animal);
            }

            GetUnlockAnimalsReply.Send(unLockAnimals);

        }
    }
}
