using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Author: Henri Leppänen
 * <summary>
 * Handles Placed Sticker rotation and scaling with user touch inputs.
 * </summary>
 */

public class StickerRotator : MonoBehaviour
{
    /// <summary>
    /// Value of added rotation speed regarding to the distance between touches to counter balance the difference of needed motion.
    /// </summary>
    [Range(0.001f, 0.008f)]
    public float rotateDistanceMultiplier = 0.005f;
    /// <summary>
    /// Minium rotation ammount when rotating, used with rotateDistanceMultiplier.
    /// </summary>
    [Range(1f,5f)]
    public float minRotationAmmount = 2.5f;

    // Reference to the sticker information that is being controlled.
    private PlacedSticker sticker;
    // Reference to the controlled sticker's rotation value.
    private Quaternion m_rotation;
    // Minimium accepted scale of the sticker.
    private float m_minScale = 0.5f;
    // Maximium accepted scale of the sticker.
    private float m_maxScale = 1f;
    // Current scale of the sticker.
    private float m_currentScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_rotation = transform.rotation;
        sticker = GetComponent<PlacedSticker>();
        PlayerInput.Instance.PinchTouchUI += CalculateNewScale;
        PlayerInput.Instance.RotateTouchUI += CalculateNewRotation;
    }

    /// <summary>
    /// Rotates the object by angle and scales it with vectorlenght.
    /// </summary>
    /// <param name="angle">Ammount to rotate.</param>
    /// <param name="vectorLenght">Distance to scale with.</param>
    private void CalculateNewRotation(float angle, float vectorLenght)
    {
        if (sticker.isSelected && !sticker.destroyStarted)
        {
            vectorLenght = Mathf.Abs(vectorLenght);
            float ammount = vectorLenght * rotateDistanceMultiplier;
            if (ammount < minRotationAmmount)
            {
                ammount = minRotationAmmount;
            }
            float deltaAngle = Mathf.DeltaAngle(angle, m_rotation.z);
            transform.Rotate(0, 0, deltaAngle * ammount);
        }
    }

    /// <summary>
    /// Scales the object with given scale.
    /// </summary>
    /// <param name="scale">New scale.</param>
    private void CalculateNewScale(float scale)
    {
        if (sticker.isSelected && !sticker.destroyStarted)
        {
            m_currentScale = Mathf.Clamp(m_currentScale + (scale * 0.002f), m_minScale, m_maxScale);
            transform.localScale = Vector3.one * m_currentScale;
        }
    }
}
