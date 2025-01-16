using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePaintSurface : MonoBehaviour {
    private Texture2D workingTexture; //the texture we modify
    private Renderer mioRenderer; //renderer component
    Texture2D texture;
    Color32 baseColor = new Color32 (255,0,0,255);

    void Start () {
        mioRenderer = GetComponent<Renderer>();
        texture = mioRenderer.material.mainTexture as Texture2D;
        workingTexture = new Texture2D (texture.width, texture.height);

        Color32[] sourcePixels = texture.GetPixels32();
        // for (int x=0; x < texture.width; x++)
        //     for (int y=0; y < texture.height; y++)
        //         workingTexture.SetPixel(x,y,baseColor);
        workingTexture.SetPixels32(sourcePixels);
        mioRenderer.material.mainTexture = workingTexture;
        workingTexture.Apply();
    }
	
    void OnParticleCollision(GameObject other) {
        int num = other.GetComponent<ParticleSystem>().GetSafeCollisionEventSize();
        ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[num];
        int result = other.GetComponent<ParticleSystem>().GetCollisionEvents(gameObject, collisionEvents);

        RaycastHit hit;
        Vector3 pos = Vector3.zero;
        Vector2 pixelUV;
        Vector2 pixelPoint;
        for (int i=0; i < num; i++){
            if (Physics.Raycast (collisionEvents [i].intersection, -Vector3.up, out hit)) {
                pos = hit.point;
                pixelUV = hit.textureCoord;
                pixelPoint = new Vector2(pixelUV.x * texture.width,pixelUV.y * texture.height);
                workingTexture.SetPixel((int)pixelPoint.x, (int)pixelPoint.y,Color.blue);
            }
        }
        workingTexture.Apply();
    }

}