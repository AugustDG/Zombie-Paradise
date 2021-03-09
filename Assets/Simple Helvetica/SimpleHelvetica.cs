//Simple Helvetica. Copyright © 2012. Studio Pepwuper, Inc. http://www.pepwuper.com/
//email: info@pepwuper.com
//version 1.0
//Customized by Augusto Mota Pinheiro, https://augustopinheiro.ca

using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleHelvetica : MonoBehaviour
{

    [HideInInspector]
    public string text = "SIMPLE HELVETICA\n \nby Studio Pepwuper";
    [HideInInspector]
    public float characterSpacing = 4f; //spacing between the characters
    [HideInInspector]
    public float lineSpacing = 22f;
    [HideInInspector]
    public float spaceWidth = 8f; //how wide should the "space" character be?

    //box collider variables
    [HideInInspector]
    public bool boxColliderIsTrigger;

    //rigidbody variables
    public float mass = 1f;
    public float drag;
    public float angularDrag = 0.05f;
    public bool useGravity = true;
    public bool isKinematic;
    public RigidbodyInterpolation interpolation;
    public CollisionDetectionMode collisionDetection;
    public bool freezePositionX;
    public bool freezePositionY;
    public bool freezePositionZ;
    public bool freezeRotationX;
    public bool freezeRotationY;
    public bool freezeRotationZ;

    private float _charXLocation;
    private float _charYLocation;

    private Vector3 _objScale; //the scale of the parent object

    //these are used in the Update() to determine if box colliders and rigidbodies should be added during update
    [HideInInspector]
    public bool boxColliderAdded;
    [HideInInspector]
    public bool rigidbodyAdded;

    private Transform _alphabetTransform;

    private void Awake()
    {
        //disable _Alphabets and all children under it to remove them from being seen.
        _alphabetTransform = transform.Find("_Alphabets");
        _alphabetTransform.gameObject.SetActive(false);
    }

    //Reset is called when the reset button is clicked on the inspector
    private void Reset()
    {
        RemoveBoxCollider();
        RemoveRigidbody();
        GenerateText();
    }

    //stores the _Alphabet transform reference
    private void GetAlphabetTransform()
    {
        _alphabetTransform = transform.Find("_Alphabets");
    }

    //Generate New 3D Text
    public void GenerateText()
    {
        ResetText(); //reset before generating new text

        //check all letters
        foreach (var chr in text)
        {
            //dealing with linebreaks "\n"
            if (chr.ToString() == "\n")
            {
                _charXLocation = 0;
                _charYLocation -= lineSpacing;
                continue;
            }

            var childObjectName = chr.ToString();
            if (childObjectName != " ")
            {
                //special naming issue where some characters (/, .) cannot be used in .fbx files' names
                var letterToShow = childObjectName switch
                {
                    "/" => transform.Find("_Alphabets/" + "slash").gameObject,
                    "." => transform.Find("_Alphabets/" + "period").gameObject,
                    _   => transform.Find("_Alphabets/" + childObjectName).gameObject
                };

                //adds letter to the series of letter objects
                AddLetter(letterToShow, chr == 'd' || chr == 't' || chr == 'b' || chr == 'h' || chr == 'k', chr == 'q' || chr == 'g' || chr == 'y' || chr == 'p' || chr == 'j');

                //find the width of the letter used
                var mesh = letterToShow.GetComponentInChildren<MeshFilter>().sharedMesh;
                var bounds = mesh.bounds;
                _charXLocation += bounds.size.x;
            }
            else
            {
                _charXLocation += spaceWidth;
            }
        }

        //disable child objects inside _Alphabets
        _alphabetTransform.gameObject.SetActive(false);
    }


    private void AddLetter(GameObject letterObject, bool isTall = false, bool hasTail = false)
    {
        var newLetter = Instantiate(letterObject, transform.position, Quaternion.Euler(new Vector3(-270f, 0f, 180f) + transform.rotation.eulerAngles));
        newLetter.transform.parent = transform; //setting parent relationship

        //rename instantiated object
        newLetter.name = letterObject.name;

        //scale according to parent obj scale
        var localScale = newLetter.transform.localScale;
        var newScaleX = localScale.x * _objScale.x;
        var newScaleY = localScale.y * _objScale.y;
        var newScaleZ = localScale.z * _objScale.z;

        var newScaleAll = new Vector3(newScaleX, newScaleY, newScaleZ);
        localScale = newScaleAll;
        newLetter.transform.localScale = localScale;
        //------------------------------------

        //dealing with characters with a line down on the left (kerning, especially for use with multiple lines)
        if (_charXLocation == 0)
            if (newLetter.name == "B" ||
                newLetter.name == "D" ||
                newLetter.name == "E" ||
                newLetter.name == "F" ||
                newLetter.name == "H" ||
                newLetter.name == "I" ||
                newLetter.name == "K" ||
                newLetter.name == "L" ||
                newLetter.name == "M" ||
                newLetter.name == "N" ||
                newLetter.name == "P" ||
                newLetter.name == "R" ||
                newLetter.name == "U" ||
                newLetter.name == "b" ||
                newLetter.name == "h" ||
                newLetter.name == "i" ||
                newLetter.name == "k" ||
                newLetter.name == "l" ||
                newLetter.name == "m" ||
                newLetter.name == "n" ||
                newLetter.name == "p" ||
                newLetter.name == "r" ||
                newLetter.name == "u" ||
                newLetter.name == "|" ||
                newLetter.name == "[" ||
                newLetter.name == "!")
                _charXLocation += 2;

        //position the new char
        newLetter.transform.localPosition = new Vector3(_charXLocation, _charYLocation, 0);

        _charXLocation += characterSpacing; //add a small space between words
    }


    private void ResetText()
    {
        //get object scale
        _objScale = transform.localScale;

        //reset position
        _charXLocation = 0f;
        _charYLocation = 0f;

        //remove all previous created letters
        var previousLetters = GetComponentsInChildren<Transform>();
        foreach (var childTransform in previousLetters)
        {
            if (childTransform.name != "_Alphabets" && childTransform.name != transform.name && childTransform.parent.name != "_Alphabets")
            {
                if (!Application.isPlaying)
                    DestroyImmediate(childTransform.gameObject);
                else
                    Destroy(childTransform.gameObject);
            }

        }

    }


    public void AddBoxCollider()
    {
        foreach (Transform child in _alphabetTransform)
        {
            child.gameObject.AddComponent<BoxCollider>();
        }

        foreach (Transform child in transform)
        {
            if (child.name != "_Alphabets")
            {
                child.gameObject.AddComponent<BoxCollider>();
            }
        }

        boxColliderAdded = true;

        //set previously set values
        SetBoxColliderVariables();
    }

    public void RemoveBoxCollider()
    {
        foreach (Transform child in transform.Find("_Alphabets"))
        {
            DestroyImmediate(child.gameObject.GetComponent<BoxCollider>());
        }

        foreach (Transform child in transform)
        {
            if (child.name != "_Alphabets")
            {
                DestroyImmediate(child.gameObject.GetComponent<BoxCollider>());
            }
        }

        boxColliderAdded = false;

    }

    public void AddRigidbody()
    {
        if (_alphabetTransform == null) GetAlphabetTransform();

        foreach (Transform child in _alphabetTransform)
        {
            child.gameObject.AddComponent<Rigidbody>();
        }

        foreach (Transform child in transform)
        {
            if (child.name != "_Alphabets")
            {
                child.gameObject.AddComponent<Rigidbody>();
            }
        }

        rigidbodyAdded = true;

        //apply previously set values
        SetRigidbodyVariables();
    }

    public void RemoveRigidbody()
    {
        if (_alphabetTransform == null) GetAlphabetTransform();

        foreach (Transform child in _alphabetTransform)
        {
            DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());
        }

        foreach (Transform child in transform)
        {
            if (child.name != "_Alphabets")
            {
                DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());
            }
        }

        rigidbodyAdded = false;

    }

    public void SetBoxColliderVariables()
    {
        if (_alphabetTransform == null) GetAlphabetTransform();
        foreach (Transform child in _alphabetTransform)
        {
            var thisCollider = child.gameObject.GetComponent<BoxCollider>();
            if (thisCollider != null)
            {
                thisCollider.isTrigger = boxColliderIsTrigger;
            }
        }

        foreach (Transform child in transform)
        {
            var thisCollider = child.gameObject.GetComponent<BoxCollider>();
            if (child.name != "_Alphabets" && thisCollider != null)
            {
                thisCollider.isTrigger = boxColliderIsTrigger;
            }
        }

    }

    public void SetRigidbodyVariables()
    {
        if (_alphabetTransform == null) GetAlphabetTransform();
        foreach (Transform child in _alphabetTransform)
        {
            var thisRigidbody = child.gameObject.GetComponent<Rigidbody>();
            if (thisRigidbody != null)
            {
                thisRigidbody.mass = mass;
                thisRigidbody.drag = drag;
                thisRigidbody.angularDrag = angularDrag;
                thisRigidbody.useGravity = useGravity;
                thisRigidbody.isKinematic = isKinematic;
                thisRigidbody.interpolation = interpolation;
                thisRigidbody.collisionDetectionMode = collisionDetection;

                if (freezePositionX)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;

                if (freezePositionY)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;

                if (freezePositionZ)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;

                if (freezeRotationX)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                if (freezeRotationY)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;

                if (freezeRotationZ)
                    thisRigidbody.constraints = thisRigidbody.constraints | RigidbodyConstraints.FreezeRotationZ;
                else
                    thisRigidbody.constraints = thisRigidbody.constraints & ~RigidbodyConstraints.FreezeRotationZ;

            }
        }

        foreach (Transform child in transform)
        {
            var thisRigidbody = child.gameObject.GetComponent<Rigidbody>();
            if (child.name != "_Alphabets" && thisRigidbody != null)
            {
                thisRigidbody.mass = mass;
                thisRigidbody.drag = drag;
                thisRigidbody.angularDrag = angularDrag;
                thisRigidbody.useGravity = useGravity;
                thisRigidbody.isKinematic = isKinematic;
                thisRigidbody.interpolation = interpolation;
                thisRigidbody.collisionDetectionMode = collisionDetection;

                if (freezePositionX)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;

                if (freezePositionY)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;

                if (freezePositionZ)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;

                if (freezeRotationX)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                if (freezeRotationY)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;

                if (freezeRotationZ)
                    thisRigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
                else
                    thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;

            }
        }

    }

    public void ResetRigidbodyVariables()
    {
        //reset rigidbody variables
        mass = 1f;
        drag = 0f;
        angularDrag = 0.05f;
        useGravity = true;
        isKinematic = false;
        interpolation = RigidbodyInterpolation.None;
        collisionDetection = CollisionDetectionMode.Discrete;
        freezePositionX = false;
        freezePositionY = false;
        freezePositionZ = false;
        freezeRotationX = false;
        freezeRotationY = false;
        freezeRotationZ = false;

        //apply changes
        SetRigidbodyVariables();

    }

    public void ApplyMeshRenderer()
    {
        if (_alphabetTransform == null) GetAlphabetTransform();

        var selfMeshRenderer = GetComponent<MeshRenderer>();
        var selfMesherRendererCastShadows = selfMeshRenderer.shadowCastingMode;
        var selfMesherRendererReceiveShadows = selfMeshRenderer.receiveShadows;
        var selfMesherRendererSharedMaterials = selfMeshRenderer.sharedMaterials;
        var selfMesherRendererUseLightProbes = selfMeshRenderer.lightProbeUsage;
        var selfMesherRendererLightProbeAnchor = selfMeshRenderer.probeAnchor;

        Debug.Log("Applied MeshRenderer");

        foreach (Transform child in _alphabetTransform)
        {
            var thisMeshRenderer = child.gameObject.GetComponentInChildren<MeshRenderer>();
            Debug.Log(selfMeshRenderer);
            if (thisMeshRenderer != null)
            {
                thisMeshRenderer.shadowCastingMode = selfMesherRendererCastShadows;
                thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
                thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
                thisMeshRenderer.lightProbeUsage = selfMesherRendererUseLightProbes;
                thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
            }
        }

        foreach (Transform child in transform)
        {
            var thisMeshRenderer = child.gameObject.GetComponent<MeshRenderer>();
            if (thisMeshRenderer != null)
            {
                thisMeshRenderer.shadowCastingMode = selfMesherRendererCastShadows;
                thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
                thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
                thisMeshRenderer.lightProbeUsage = selfMesherRendererUseLightProbes;
                thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
            }
        }
    }

    public void DisableSelf()
    {
        enabled = false;
        //Debug.Log ("enabled? "+enabled);
    }

    public void EnableSelf()
    {
        enabled = true;
        //Debug.Log ("enabled? "+enabled);
    }
}