using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Extensions;

namespace SecuredSpace.Effects
{
    public class AnimationScript : MonoBehaviour
    {
        public List<Sprite> frames;
        public bool loop = false;
        public float speed = 0.13f;
        private SpriteRenderer rend;
        private LineRenderer lineRend;
        private int currentFrame;
        public bool playing = false;
        public bool playFrame = false;
        public bool noneSpriteAfterPlay = false;
        public bool Randomize = false;
        public bool noRepetitionRandom = true;
        public bool StopAnimation = false;
        public bool ReverseAnimation = false;
        public bool GenerateFromTexturesToSprites = false;
        public bool DiffuseSprite = false;
        [Space(10)]
        public bool RenderOnSprite = true;
        public float SpriteScaler = 1f;
        [Space(10)]
        public bool RenderOnLine;
        public Material RenderMaterial;
        public bool LineTransparentFading;
        public List<Texture2D> textureFrames;
        public System.Action<GameObject> afterPlayAction = (gameobject) => { };
        public System.Action<GameObject> onPlayAction = (gameobject) => { };
        public UnityExtend.SpriteExtensions.TransformType transformType;
        private Color lineColor;

        public void CopyToAnimation(AnimationScript animationScript)
        {
            animationScript.frames = this.frames;
            animationScript.loop = this.loop;
            animationScript.speed = this.speed;
            animationScript.noneSpriteAfterPlay = this.noneSpriteAfterPlay;
            if (loop)
            {
                animationScript.Play();
            }
        }

        public void RegenerateTextureFrames()
        {
            foreach(var rawsprite in frames)
            {
                var sprite = SecuredSpace.UnityExtend.SpriteExtensions.Duplicate(rawsprite, transformType);
                //int x = Mathf.FloorToInt(sprite.rect.x);
                //int y = Mathf.FloorToInt(sprite.rect.y);
                //int width = Mathf.FloorToInt(sprite.rect.width);
                //int height = Mathf.FloorToInt(sprite.rect.height);

                //Color[] pix = sprite.texture.GetPixels(x, y, width, height);
                //Texture2D destTex = new Texture2D(width, height);
                //destTex.SetPixels(pix);
                //destTex.Apply();
                textureFrames.Add(sprite.texture);
                
            }
            
        }

        public void RegenerateSpriteFrames()
        {
            foreach (var rawsprite in textureFrames)
            {
                frames.Add(SecuredSpace.UnityExtend.SpriteExtensions.TextureToSprite(rawsprite));
            }
        }

        public void Awake()
        {
            Initialize();
        }
        private bool inited = false;
        public void Initialize()
        {
            if(!inited)
            {
                if (RenderOnSprite)
                {
                    rend = GetComponent<SpriteRenderer>();
                    if(DiffuseSprite)
                        rend.material.shader = Shader.Find("Sprites/Diffuse");
                }
                if (GenerateFromTexturesToSprites)
                    RegenerateSpriteFrames();
                if (ReverseAnimation)
                    frames.Reverse();
                if (RenderOnLine)
                {
                    RegenerateTextureFrames();
                    lineRend = GetComponent<LineRenderer>();
                    if (textureFrames.Count > 0)
                        lineRend.material.mainTexture = textureFrames[0];
                }
                inited = true;
                if (StopAnimation)
                    StopAnimation = false;
                else
                    Play();
            }
        }

        public void Update()
        {
            if (!playFrame && playing && inited)
                Play();
            if (LineTransparentFading && inited)
                FadeUpdate();
        }
        
        public void Play()
        {
            if (textureFrames.Count == 0 && frames.Count == 0)
                return;
            playFrame = true;
            playedMs = 0;
            this.transform.localScale = new Vector3(SpriteScaler, SpriteScaler, SpriteScaler);
            this.StopAllCoroutines();
            StopAnimation = false;
            onPlayAction(this.gameObject);
            StartCoroutine(PlayAnim());
        }
        float playedMs = 0;
        void FadeUpdate()
        {
            if (playing)
            {
                playedMs += Time.deltaTime;
                var percentFade = playedMs / (double)(frames.Count * speed);
                var cColor = new Color(lineColor.r, lineColor.g, lineColor.b, 1f - (float)percentFade);
                lineRend.material.SetColor("_Color", cColor);
            }
        }

        IEnumerator PlayAnim()
        {
            playing = true;
            if (RenderOnLine)
            {
                if (lineColor == Color.clear)
                    lineColor = lineRend.material.GetColor("_Color");
                lineRend.material.SetColor("_Color", lineColor);
            } 
            currentFrame = 0;
            if (!StopAnimation)
            {
                if(lineRend != null)
                    lineRend.enabled = true;
                if (Randomize)
                {

                }
                else
                {
                    if (rend != null)
                        rend.sprite = frames[currentFrame];
                    if(lineRend != null)
                        lineRend.material.mainTexture = textureFrames[currentFrame];
                    currentFrame++;
                    yield return new WaitForSeconds(speed);
                }
                
            }
            

            while (currentFrame < frames.Count)
            {
                if (StopAnimation)
                    break;
                yield return new WaitForSeconds(speed);
                if (Randomize)
                {
                    int randomFrame = Random.Range(0, frames.Count);
                    if (noRepetitionRandom)
                    {
                        currentFrame = randomFrame == currentFrame ? (randomFrame > 0 ? randomFrame - 1 : randomFrame + 1) : randomFrame;
                        if (rend != null)
                            rend.sprite = frames[currentFrame];
                        if (lineRend != null)
                            lineRend.material.mainTexture = textureFrames[currentFrame];
                    }
                    else
                    {
                        currentFrame = randomFrame;
                        if (rend != null)
                            rend.sprite = frames[currentFrame];
                        if (lineRend != null)
                            lineRend.material.mainTexture = textureFrames[currentFrame];
                    }
                }
                else
                {
                    if (lineRend != null)
                        lineRend.material.mainTexture = textureFrames[currentFrame];
                    if (rend != null)
                        rend.sprite = frames[currentFrame];
                    currentFrame++;
                }
                    
                
                //Debug.Log("frame: " + currentFrame);
            }
            if (noneSpriteAfterPlay)
            {
                yield return new WaitForSeconds(speed);
                if (rend != null)
                    rend.sprite = null;
                if (lineRend != null)
                {
                    lineRend.material.mainTexture = null;
                    lineRend.enabled = false;
                }
            }

            afterPlayAction(this.gameObject);

            if (loop && !StopAnimation)
                playing = true;//Play();
            else
                playing = false;
            playFrame = false;
        }
    }

}