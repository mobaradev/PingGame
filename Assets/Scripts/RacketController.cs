using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketController : MonoBehaviour
{
    public Color color = Color.red;
    public float r;
    public float g;
    public float b;
    /*
        Each component of the color (r, g, b) is changed independently
        and can be increasing (up to 255) or decreasing (to 0)
    */
    private bool _isRIncreasing = true;
    private bool _isGIncreasing = true;
    private bool _isBIncreasing = true;
    
    public Material paddleMaterial;
    
    void Start()
    {
        MeshCollider mesh = this.GetComponentInChildren<MeshCollider>();
        var size = mesh.bounds.size;
        this.r = this.color.r;
        this.g = this.color.g;
        this.b = this.color.b;
        Debug.Log("Mesh:");
        Debug.Log(size);
    }
    
    void Update()
    {
        MeshCollider mesh = this.GetComponentInChildren<MeshCollider>();
        var size = mesh.bounds.size;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mousePos.z = 7.8f/2;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.z = -7.8f;

        this.transform.position = worldPosition;
        this.HandleRotation();
    }

    private void FixedUpdate()
    {
        this.HandleColorAutoChange();
    }

    private void HandleColorAutoChange()
    {
        if (this._isRIncreasing)
        {
            this.r += 0.01f;
        }
        else this.r -= 0.01f;
        
        if (this.r >= 1.0f)
        {
            this.r = 1.0f;
            this._isRIncreasing = false;
        } else if (this.r <= 0.0f)
        {
            this.r = 0.0f;
            this._isRIncreasing = true;
        }
        
        if (this._isGIncreasing)
        {
            this.g += 0.025f;
        } else this.g -= 0.025f;
        
        if (this.g >= 1.0f)
        {
            this.g = 1.0f;
            this._isGIncreasing = false;
        } else if (this.g <= 0.0f)
        {
            this.g = 0.0f;
            this._isGIncreasing = true;
        }
        
        if (this._isBIncreasing)
        {
            this.b += 0.014f;
        } else this.b -= 0.014f;
        
        if (this.b >= 1.0f)
        {
            this.b = 1.0f;
            this._isBIncreasing = false;
        } else if (this.b <= 0.0f)
        {
            this.b = 0.0f;
            this._isBIncreasing = true;
        }

        this.color = new Color(this.r, this.g, this.b, 0.86f);
        this.paddleMaterial.SetColor("_Color", this.color);
    }

    void HandleRotation()
    {
        float positionX = this.transform.position.x;
        
        this.transform.eulerAngles = new Vector3(
            positionX/0.1f,
            this.transform.eulerAngles.y,
            this.transform.eulerAngles.z
        );

    }
}
