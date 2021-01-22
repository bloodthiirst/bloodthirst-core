using Assets.Scripts.ChatUI;
using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Game.Chat.Processors;
using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManagerUI : UnitySingleton<ChatManagerUI>
{
    private ChatMessagePacketClientProcessor socketChatMessage;

    [SerializeField]
    private RectTransform container;

    [SerializeField]
    private ChatEntryBehaviour chatPrefab;

    [SerializeField]
    private Scrollbar chatScrollbar;


    [SerializeField]
    private ChatMessageNetworkBehaviour currentMessageNetworkBehaviour;


    [SerializeField]
    private TMP_InputField chatInput;

    [SerializeField]
    private bool chatIsSelected;

    public ChatMessageNetworkBehaviour CurrentMessageNetworkBehaviour => currentMessageNetworkBehaviour;

    protected override void Awake()
    {
        base.Awake();

        chatInput.onSelect.AddListener(OnChatInputSelected);
        chatInput.onDeselect.AddListener(OnChatInputDeselected);
    }

    private void OnChatInputDeselected(string arg0)
    {
        chatIsSelected = false;

        //ContextSystemManager.RepalceContext(ChatTypingGameContext.Instance, GameplayInputGameContext.Instance);
    }

    private void OnChatInputSelected(string arg0)
    {
        chatIsSelected = true;

        //ContextSystemManager.RepalceContext(GameplayInputGameContext.Instance, ChatTypingGameContext.Instance);
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatIsSelected)
            {
                Send();
            }

            EventSystem.current.SetSelectedGameObject(chatInput.gameObject, null);
            chatInput.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (chatIsSelected)
            {
                EventSystem.current.SetSelectedGameObject(null, null);
                chatInput.OnDeselect(new PointerEventData(EventSystem.current));
            }
        }
    }


    public void Setup(ChatMessageNetworkBehaviour chatMessageNetwork)
    {
        currentMessageNetworkBehaviour = chatMessageNetwork;
    }

    [Button]
    public void Send()
    {
        if (CurrentMessageNetworkBehaviour == null)
            return;

        CurrentMessageNetworkBehaviour.SendChatMessage("Bloodthirst", chatInput.text);

        chatInput.text = string.Empty;
    }


    public ChatEntryBehaviour AppendMessage()
    {
        ChatEntryBehaviour chatGO = Instantiate(chatPrefab, container);

        StartCoroutine(ChangeScrollbarPosition());

        return chatGO;
    }


    private IEnumerator ChangeScrollbarPosition()
    {
        yield return null;

        yield return null;

        yield return null;

        chatScrollbar.value = 0f;

        yield return 0;
    }
}