using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionRank : MonoBehaviour
{
    [SerializeField] List<Image> starList;

    private void Reset()
    {
        starList = GetComponentsInChildren<Image>().ToList<Image>();
        starList.Remove(this.GetComponent<Image>());
    }

    public void SetRank(int _Rank)
    {
        for (int i = 0; i < starList.Count; i++)
        {
            starList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _Rank; i++)
        {
            starList[i].gameObject.SetActive(true);
        }
    }
}
