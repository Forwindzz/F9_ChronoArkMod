using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeReferences : MonoBehaviour
{
    public Library_SpriteStudio6.Data.CellMap cellMap;
    public Library_SpriteStudio6.Data.Animation animation_;
    public Library_SpriteStudio6.Data.Effect.Emitter effect;
    public Library_SpriteStudio6.Data.Parts.Animation parts_anim;
    public Library_SpriteStudio6.Data.Parts.Effect parts_effect;
    public Library_SpriteStudio6.Control.AdditionalColor additionalColor;
    public Library_SpriteStudio6.Control.Animation.Parts animation_2_parts;
    public Library_SpriteStudio6.Control.Effect effect_;
    public Library_SpriteStudio6.Script.Root root;
    public Script_SpriteStudio6_DataAnimation.DataSetup data_setup;
    public Script_SpriteStudio6_Root.InformationPlay info;


    // Start is called before the first frame update
    void Start()
    {
        System.Type a = typeof(Library_SpriteStudio6.Data.CellMap);
        Debug.Log(a);
    }
}
