using Bloodthirst.BDeepCopy;
using Bloodthirst.Runtime.BRecorder;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEngine;

public class BRecorderBehaviour : MonoBehaviour
{
    [SerializeField]
    public BRecorderAsset recorderAsset;

    private BRecorderAsset assetCopy;

    [SerializeField]
    public bool playOnAwake;

    [SerializeField]
    #if ODIN_INSPECTOR[ReadOnly]#endif
    private bool isPlaying;

    private int nextCommandIndex;

    private IBRecorderCommand nextCommand;



    private void Awake()
    {
        if (!playOnAwake)
            return;

        nextCommandIndex = 0;
        isPlaying = true;
        nextCommand = recorderAsset.Session.Commands[nextCommandIndex];

        assetCopy = BCopier<BRecorderAsset>.Instance.Copy(recorderAsset);

        foreach (IBRecorderCommand cmd in assetCopy.Session.Commands)
        {
            cmd.Setup();
        }
    }

    private void Update()
    {
        while ((nextCommand != null) && (nextCommand.GameTime <= Time.time) && (isPlaying))
        {
            nextCommand.PreExecute();
            nextCommand.Execute();

            nextCommandIndex++;

            if (nextCommandIndex > assetCopy.Session.Commands.Count - 1)
            {
                isPlaying = false;
                return;
            }

            nextCommand = assetCopy.Session.Commands[nextCommandIndex];
        }


    }


}
