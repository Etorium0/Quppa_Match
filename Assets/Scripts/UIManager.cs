using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
	public delegate void Action();

	[Serializable]
	public class Page
	{
		public string name;

		public List<UIPanel> panels = new List<UIPanel>();

		public List<UIPanel> ignoring_panels = new List<UIPanel>();

		public string soundtrack = "-";

		public bool show_ads;

		public bool default_page;

		public bool setTimeScale = true;

		public float timeScale = 1f;
	}

	public List<Transform> UImodules = new List<Transform>();

	public static Action<string> onShowPage = delegate
	{
	};

	public List<UIPanel> panels = new List<UIPanel>();

	public UIPanel panelsStore;

	public List<Page> pages = new List<Page>();

	private string currentPage;

	private string previousPage;



	private void Start()
	{
		Application.targetFrameRate = 60;
		ArraysConvertation();
		Page defaultPage = GetDefaultPage();
		if (defaultPage != null)
		{
			ShowPage(defaultPage, immediate: true);
		}
	}

	private void OnDisable()
	{
	}

	public void OpenStore()
	{
		
		
	}

	

	public void ArraysConvertation()
	{
		panels = new List<UIPanel>();
		panels.AddRange(GetComponentsInChildren<UIPanel>(includeInactive: true));
		foreach (Transform uImodule in UImodules)
		{
			if (uImodule != null && uImodule.GetComponentsInChildren<UIPanel>(includeInactive: true) != null)
			{
				panels.AddRange(uImodule.GetComponentsInChildren<UIPanel>(includeInactive: true));
			}
		}
		if (Application.isEditor)
		{
			panels.Sort((UIPanel a, UIPanel b) => string.Compare(a.name, b.name));
		}
	}

	public void ShowPage(Page page, bool immediate = false)
	{
		if (!(currentPage == page.name) && pages != null)
		{
			previousPage = currentPage;
			currentPage = page.name;
			foreach (UIPanel panel in panels)
			{
				if (page.panels.Contains(panel))
				{
					panel.SetVisible(visible: true, immediate);
				}
				else if (!page.ignoring_panels.Contains(panel) && !panel.freez)
				{
					panel.SetVisible(visible: false, immediate);
				}
			}
			onShowPage(page.name);
			if (page.soundtrack != "-" && page.soundtrack != SingletonMonoBehaviour<AudioManager>.Instance.currentTrack)
			{
				SingletonMonoBehaviour<AudioManager>.Instance.PlayMusic(page.soundtrack);
			}
			if (page.setTimeScale)
			{
				Time.timeScale = page.timeScale;
			}
		}
	}

	public void ShowPage(string page_name)
	{
		ShowPage(page_name, immediate: false);
	}

	public void ShowPage(string page_name, bool immediate)
	{
		Page page = pages.Find((Page x) => x.name == page_name);
		if (page != null)
		{
			ShowPage(page, immediate);
		}
	}

	public void FreezPanel(string panel_name, bool value = true)
	{
		UIPanel uIPanel = panels.Find((UIPanel x) => x.name == panel_name);
		if (uIPanel != null)
		{
			uIPanel.freez = value;
		}
	}

	public void SetPanelVisible(string panel_name, bool visible, bool immediate = false)
	{
		UIPanel uIPanel = panels.Find((UIPanel x) => x.name == panel_name);
		if ((bool)uIPanel)
		{
			if (immediate)
			{
				uIPanel.SetVisible(visible, immediate: true);
			}
			else
			{
				uIPanel.SetVisible(visible);
			}
		}
	}

	public void HideAll()
	{
		foreach (UIPanel panel in panels)
		{
			panel.SetVisible(visible: false);
		}
	}

	public void HideStoresPage(){

        panelsStore.SetVisible(visible: false);
		currentPage = "MissionPage";
		
	}

	public void HideStoresPageGamePlay(){
        panelsStore.SetVisible(visible: false);
        previousPage = "GamePage";
		ShowPage(previousPage);
		SingletonMonoBehaviour<GameMgr>.Instance.ResumeGame();

    }
	public void ShowPreviousPage()
	{
		ShowPage(previousPage);
	}

	public string GetCurrentPage()
	{
		return currentPage;
	}

	public Page GetDefaultPage()
	{
		return pages.Find((Page x) => x.default_page);
	}
}
