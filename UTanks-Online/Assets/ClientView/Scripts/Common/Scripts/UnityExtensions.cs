using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SecuredSpace.UnityExtend
{
    public static class SpriteExtensions
    {
        [System.Serializable]
        public enum TransformType
        {
            None,
            Rotate90Clockwise,
            Rotate90CounterClockwise,
            Rotate180,
            FlipHorizontal,
            FlipVertical
        }

        public static Sprite TextureToSprite(Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
        public static Sprite Duplicate(this Sprite originalSprite, TransformType transform = TransformType.None)
        {
            if (originalSprite == null) throw new System.Exception("Duplicate called on null sprite");

            Sprite duplicate = null;

            int x = Mathf.FloorToInt(originalSprite.rect.x);
            int y = Mathf.FloorToInt(originalSprite.rect.y);
            int width = Mathf.FloorToInt(originalSprite.rect.width);
            int height = Mathf.FloorToInt(originalSprite.rect.height);

            Color[] originalPixels = originalSprite.texture.GetPixels(x, y, width, height);

            Color[] transformedPixels = null;

            switch (transform)
            {
                case TransformType.None:
                    transformedPixels = originalPixels;
                    break;
                case TransformType.Rotate90Clockwise:
                    {
                        transformedPixels = new Color[originalPixels.Length];

                        for (int segment = 0; segment < transformedPixels.Length; segment += height)
                        {
                            for (int offset = 0; offset < height; offset++)
                            {
                                int pair = (offset * width) + (width - 1) - (segment / height);

                                transformedPixels[segment + offset] = originalPixels[pair];
                            }
                        }

                        int temp = width;

                        width = height;

                        height = temp;
                    }
                    break;
                case TransformType.Rotate90CounterClockwise:
                    {
                        transformedPixels = new Color[originalPixels.Length];

                        for (int segment = 0; segment < transformedPixels.Length; segment += height)
                        {
                            for (int offset = 0; offset < height; offset++)
                            {
                                int pair = (transformedPixels.Length - 1) - (offset * width) - (width - 1) + (segment / height);

                                transformedPixels[segment + offset] = originalPixels[pair];
                            }
                        }

                        int temp = width;

                        width = height;

                        height = temp;
                    }
                    break;
                case TransformType.Rotate180:
                    transformedPixels = originalPixels;

                    System.Array.Reverse(transformedPixels);
                    break;
                case TransformType.FlipHorizontal:
                    transformedPixels = originalPixels;

                    for (int segmentStart = 0; segmentStart < transformedPixels.Length; segmentStart += width)
                    {
                        System.Array.Reverse(transformedPixels, segmentStart, width);
                    }
                    break;
                case TransformType.FlipVertical:
                    transformedPixels = new Color[originalPixels.Length];

                    for (int leftSegment = 0, rightSegment = transformedPixels.Length - width;
                        leftSegment < rightSegment;
                        leftSegment += width, rightSegment -= width)
                    {

                        for (int adjustment = 0; adjustment < width; adjustment++)
                        {
                            int a = leftSegment + adjustment;
                            int b = rightSegment + adjustment;

                            transformedPixels[a] = originalPixels[b];
                            transformedPixels[b] = originalPixels[a];
                        }
                    }
                    break;
            }

            if (transformedPixels != null)
            {
                Texture2D texture = new Texture2D(width, height);

                texture.SetPixels(transformedPixels);
                texture.Apply();

                duplicate = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit);
            }

            return duplicate;
        }
    }

    public static class UnityExtensions
    {
        public static Component CopyComponent(this Component original, GameObject destination)
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                try { field.SetValue(copy, field.GetValue(original)); } catch { }
            }

            System.Reflection.PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly);
            foreach (System.Reflection.PropertyInfo field in properties)
            {
                try { field.SetValue(copy, field.GetValue(original)); } catch { }
            }
            return copy;
        }

        public static T CopyComponent<T>(this T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);

            System.Reflection.FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (System.Reflection.FieldInfo field in fields)
            {
                try { field.SetValue(copy, field.GetValue(original)); } catch { }
            }

            System.Reflection.PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly);
            foreach (System.Reflection.PropertyInfo field in properties)
            {
                try { field.SetValue(copy, field.GetValue(original)); } catch { }
            }
            return copy as T;
        }
    }

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<DictionaryNode<TKey, TValue>> dictionary = new List<DictionaryNode<TKey, TValue>>();

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            DirectUpdate();
        }

        public new void Remove(TKey key)
        {
            base.Remove(key);
            DirectUpdate();
        }

        public new void Clear()
        {
            base.Clear();
            DirectUpdate();
        }

        private void DirectUpdate()
        {
            dictionary.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                dictionary.Add(new DictionaryNode<TKey, TValue>(pair.Key, pair.Value));
            }
        }

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            if (dictionary.Count == this.Count && dictionary.Count == 0)
                return;
            if (dictionary.Count == this.Count)
            {
                dictionary.Clear();
                foreach (KeyValuePair<TKey, TValue> pair in this)
                {
                    dictionary.Add(new DictionaryNode<TKey, TValue>(pair.Key, pair.Value));
                }
            }
            else
            {
                Debug.Log("You have a double key elements in data array. Remove or rename key on this data");
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            base.Clear();

            //while(keys.Count > values.Count)
            //    values.Add(default(TValue));
            //while (keys.Count < values.Count)
            //    keys.Add(default(TKey));
            //throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
            for (int i = 0; i < dictionary.Count; i++)
            {
                try
                {
                    base.Add(dictionary[i].key, dictionary[i].value);
                }
                catch
                {

                }
            }
        }
    }
    [System.Serializable]
    public class DictionaryNode<TKey, TValue>
    {
        public TKey key;
        public TValue value;
        public DictionaryNode(TKey Key, TValue Value)
        {
            key = Key;
            value = Value;
        }
    }

    [System.Serializable]
    public class Characteristic
    {
        public string FullName;
        public float Value;
        public bool Constant;
        public bool Visibility;
        public Texture2D Icon;
    }

    [System.Serializable]
    public class NoSingleObjects<T>
    {
        public T Value;
        public float Count;
        public bool Infinity;
    }

    public struct LayerMaskEx
    {
        public static bool PresentedInLayerMask(LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }
    }
}
