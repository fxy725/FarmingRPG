using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    [SerializeField] private UIInventoryBar uiInventoryBar;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject[] menuTabs;
    [SerializeField] private Button[] menuButtons;

    private bool _pauseMenuOn;
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }



    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }



    private void PauseMenu()
    {
        // 按下ESC键时，暂停菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        // 销毁当前拖拽的物品
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        // 清空当前选中的物品
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        // 触发垃圾回收
        System.GC.Collect();

        // 高亮选中的按钮
        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        // 销毁当前拖拽的物品
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }

            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
