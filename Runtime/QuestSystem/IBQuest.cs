using System;

namespace Bloodthirst.System.Quest
{
    public enum QUEST_STATUS
    {

        /// <summary>
        /// The quest is inactive and not reached yet
        /// </summary>
        INACTIVE,

        /// <summary>
        /// The quest is active a being tracked
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The quest is waiting for a step to continue : player choice , a specific event , etc ...
        /// </summary>
        PENDING,

        /// <summary>
        /// The quest is done
        /// </summary>
        DONE
    }

    public interface IBQuest<TQuestData, TQuestType>
        where TQuestType : IBQuest<TQuestData, TQuestType>
    {
        event Action<TQuestType> OnQuestStatusChanged;

        event Action<TQuestType> OnStateChanged;

        TQuestData Identifier { get; }
        
        QUEST_STATUS QuestStatus { get;}

        void Intialize();

        void Destroy();

    }
}
