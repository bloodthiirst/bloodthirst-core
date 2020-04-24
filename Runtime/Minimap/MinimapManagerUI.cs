using Assets.Scripts.GameEvent;
using Assets.Scripts.Minimap;
using Assets.Scripts.Utils;
using Bloodthirst.Core.UnitySingleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MinimapManagerUI : UnitySingleton<MinimapManagerUI>, IPointerClickHandler
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Camera minimapCamera;

    [SerializeField]
    private RectTransform minimapRectTransform;

    [SerializeField]
    private RawImage minimapRender;

    private RenderTexture mapTex;

    [SerializeField]
    private bool rotateWithTarget;

    [SerializeField]
    private float rotationOffset;

    private RaycastHit[] raycastHits = new RaycastHit[20];



    private void IniTexture()
    {
        mapTex = new RenderTexture(512, 512, 24);
        mapTex.enableRandomWrite = true;
        mapTex.Create();
    }

    private void Awake()
    {
        IniTexture();

        minimapCamera.targetTexture = mapTex;

        minimapRender.texture = mapTex;

        OnMinimapClickGameEvent.Register(OnClickTest);
    }

    public void Bind(Transform target)
    {
        this.target = target;
    }

    private void OnClickTest(MinimapClickArgs obj)
    {
        Debug.Log("Gameobject hit in minimap : " + obj.ClickedObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRectTransform, eventData.position, null, out localPoint);

        Vector2 normalizedPoint;

        normalizedPoint.x = localPoint.x / minimapRectTransform.rect.width;
        normalizedPoint.y = localPoint.y / minimapRectTransform.rect.height;

        Debug.Log("normalizedPoint value is : " + normalizedPoint);


        Ray rayToWorld = minimapCamera.ViewportPointToRay(normalizedPoint);

        int hitCount = Physics.RaycastNonAlloc(rayToWorld, raycastHits);

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = raycastHits[i];

            OnMinimapClickGameEvent.Invoke( new MinimapClickArgs() { ClickedObject = hit.transform.gameObject } );
        }

    }

    private void Update()
    {
        if (target == null)
            return;

        if (rotateWithTarget)
        {
            minimapCamera.transform.rotation = Quaternion.Euler(90, 0, -target.rotation.eulerAngles.y + rotationOffset);
        }
        
        else
        {
            minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        minimapCamera.transform.position = target.position.Change(null, minimapCamera.transform.position.y);
    }

    private void OnDestroy()
    {
        mapTex.Release();
    }

    private void OnApplicationQuit()
    {
        mapTex.Release();
    }
}
