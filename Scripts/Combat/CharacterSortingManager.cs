using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class CharacterSortingManager : MonoBehaviour
{
    public static CharacterSortingManager Instance { get; private set; }

    private List<BaseCombatAI> characters = new List<BaseCombatAI>();
    private const int BaseOrder = 1000; // 기본 정렬 순서
    private const int OrderStep = 10; // 정렬 순서 간격

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        SortCharactersByYPosition();
    }

    public void RegisterCharacter(BaseCombatAI character)
    {
        if (!characters.Contains(character))
        {
            characters.Add(character);
        }
    }

    public void UnregisterCharacter(BaseCombatAI character)
    {
        characters.Remove(character);
    }

    private void SortCharactersByYPosition()
    {
        var sortedCharacters = characters
            .Where(c => c != null && c.gameObject.activeSelf)
            .OrderByDescending(c => c.transform.position.y)
            .ToList();

        for (int i = 0; i < sortedCharacters.Count; i++)
        {
            var character = sortedCharacters[i];
            var sortingGroup = character.GetComponentInChildren<SortingGroup>();
            if (sortingGroup != null)
            {
                sortingGroup.sortingOrder = BaseOrder + i * OrderStep;
            }
        }
    }
}