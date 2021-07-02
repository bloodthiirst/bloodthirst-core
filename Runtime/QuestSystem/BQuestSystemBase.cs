using Bloodthirst.Core.TreeList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quest
{
    public class BQuestSystemBase<TType , TData> 
        where TType : class , IBQuest<TData,TType>
    {
        private TreeList<TData, TType> QuestTree { get; set; }

        public void Refresh()
        {

        }

        public void Initialize()
        {

        }

        public void Destroy()
        {

        }
    }
}
