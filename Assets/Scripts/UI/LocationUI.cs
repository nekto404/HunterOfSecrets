using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationUI : MonoBehaviour
{
    [Header("Question")]
    [SerializeField] private GameObject QuestionUI;
    [SerializeField] private Button yesButtonQuestionUI;    
    [SerializeField] private Button noButtonQuestionUI;    
    [SerializeField] private TMP_Text messageTextQuestionUI;
    private List<UnityEvent> yesEventsQuestionUI;           
    private List<UnityEvent> noEventsQuestionUI;         
    
    [Header("TextResult")]
    [SerializeField] private GameObject TextResultUI;
    [SerializeField] private Button skipButtonTextResultUI;
    [SerializeField] private TMP_Text messageTextResultUI;

    [Header("PathUI")]
    [SerializeField] private GameObject PathUI;
    [SerializeField] private ScrollController scrollControllerPathUI;    
    [SerializeField] private GameObject tilePrefabPathUI;

    [Header("ItemRewardUI")]
    [SerializeField] private GameObject ItemRewardUI;
    [SerializeField] private Transform placeForItemRewardUI;
    [SerializeField] private  GameObject itemPrefabRewardUI;

    [Header("ActionChoseUI")]
    [SerializeField] private GameObject ActionChoseUI;
    [SerializeField] private Button firstButtonQuestionUI;   
    [SerializeField] private Button secondButtonQuestionUI;
    private List<UnityEvent> firstEventsQuestionUI;           
    private List<UnityEvent> secondEventsQuestionUI;          
}
