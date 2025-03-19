
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private ApollonianGasketSprites spriteFractalCs;
    private ApollonianGasketFractal lineRendererCs;
    [SerializeField] private GameObject spriteGameObject;
    [SerializeField] private GameObject lineRendererGameObject;
    private enum FractalColor
    {
        White,
        Red,
        Green,
        Blue,
        Yellow,
    }
    
    [SerializeField] private FractalColor fractalColor;
    [SerializeField] private TMP_Dropdown fractalColorDropdown;
    [SerializeField] private Button generateButton;
    [SerializeField] private Button lineRendererFractalButton;
    [SerializeField] private Button spriteFractalButton;
    

    private void Awake()
    {
        spriteFractalCs = spriteGameObject.GetComponent<ApollonianGasketSprites>();
        lineRendererCs = lineRendererGameObject.GetComponent<ApollonianGasketFractal>();
    }

    private void Start()
    {
        fractalColorDropdown.onValueChanged.AddListener(delegate
        {
            fractalColor = (FractalColor) fractalColorDropdown.value;
        });
        
        generateButton.onClick.AddListener(() =>
        {
            Color selectedColor = GetColorFromEnum(fractalColor);
            if (spriteGameObject.activeSelf)
            {
                spriteFractalCs.circleColor = selectedColor;
                spriteFractalCs.CreateNewCircles();
            }
            else
            {
                lineRendererCs.lineColor = selectedColor;
                lineRendererCs.CreateNewCircles();
            }
        });
        
        lineRendererFractalButton.onClick.AddListener(() =>
        {
            spriteGameObject.SetActive(false);
            lineRendererGameObject.SetActive(true);
        });
        
        spriteFractalButton.onClick.AddListener(() =>
        {
            spriteGameObject.SetActive(true);
            lineRendererGameObject.SetActive(false);
        });
    }

    private Color GetColorFromEnum(FractalColor color)
    {
        switch (color)
        {
            case FractalColor.White: return Color.white;
            case FractalColor.Red: return Color.red;
            case FractalColor.Green: return Color.green;
            case FractalColor.Blue: return Color.blue;
            case FractalColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }
}
