using UnityEngine;
using TMPro;

public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshTop1;
    [SerializeField] private TextMeshProUGUI textMeshTop2;
    [SerializeField] private TextMeshProUGUI textMeshTop3;
    [SerializeField] private TextMeshProUGUI textMeshBottom1;
    [SerializeField] private TextMeshProUGUI textMeshBottom2;
    [SerializeField] private TextMeshProUGUI textMeshBottom3;



    // 设置文本框文本
    public void SetTextboxText(string textTop1, string textTop2, string textTop3, string textBottom1, string textBottom2, string textBottom3)
    {
        textMeshTop1.text = textTop1;
        textMeshTop2.text = textTop2;
        textMeshTop3.text = textTop3;
        textMeshBottom1.text = textBottom1;
        textMeshBottom2.text = textBottom2;
        textMeshBottom3.text = textBottom3;
    }

}
