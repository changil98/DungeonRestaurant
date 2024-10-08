using UnityEngine;

public class CharacterNameList
{
    public string GetRandomName(eCharacterRank rank)
    {
        string[] list;
        switch (rank)
        {
            case eCharacterRank.Hero:
                list = list_Hero;
                break;
            case eCharacterRank.Rare:
                list = list_Rare;
                break;
            default:
                list = list_Common;
                break;
        }
        return list[Random.Range(0, list.Length)];
    }

    [SerializeField]
    string[] list_Common = new string[]
    {
        "홍길동",
        "이덕배",
        "박춘자",
        "김철수",
        "조영희",
        "아르노",
        "볼드윈",
        "카이론",
        "에드릭",
        "다리우스",
        "하몬",
        "카다스",
        "리오란",
        "페론",
        "그레이든",
        "베른",
        "이안",
        "제로스",
        "페릭스",
        "다스티온",
        "엘라스",
        "알버트",
        "가로드",
        "크리스탄",
        "세르노",
        "카스티엘",
        "다스탄",
        "아일런",
        "볼테론",
        "도란",
        "자이런",
        "에리사",
        "리에나",
        "메릴린",
        "니케아",
        "자레나",
        "리오나",
        "세피라",
        "카산드라",
        "아멜리아",
        "루미아",
        "페라니아",
        "엘리나",
        "노에라",
        "카트린",
        "루미엘",
        "안타드라",
        "셀시우스",
        "자렐라",
        "오렌시아",
        "엘바딘",
        "로자나",
        "브라이스",
        "아이릭",
        "아이젠",
        "가렌",
        "시오렌",
        "자비에르",
        "에마레스",
        "드레이크",
        "코비",
        "카스란",
        "조로",
        "엘리나스",
        "타리사",
        "나디아",
        "카르마"
    };
    [SerializeField]
    string[] list_Rare = new string[]
    {
        "프로도",
        "간달프",
        "레골라스",
        "골룸",
        "사우론",
        "멀린",
        "류",
        "켄",
        "춘리",
        "루크",
        "아르노",
        "볼드윈",
        "카이론",
        "에드릭",
        "다리우스",
        "하몬",
        "카다스",
        "리오란",
        "페론",
        "그레이든",
        "베른",
        "이안",
        "제로스",
        "페릭스",
        "다스티온",
        "엘라스",
        "알버트",
        "가로드",
        "크리스탄",
        "세르노",
        "카스티엘",
        "다스탄",
        "아일런",
        "볼테론",
        "도란",
        "자이런",
        "에리사",
        "리에나",
        "메릴린",
        "니케아",
        "자레나",
        "리오나",
        "세피라",
        "카산드라",
        "아멜리아",
        "루미아",
        "페라니아",
        "엘리나",
        "노에라",
        "카트린",
        "루미엘",
        "안타드라",
        "셀시우스",
        "자렐라",
        "오렌시아",
        "엘바딘",
        "로자나",
        "브라이스",
        "아이릭",
        "아이젠",
        "가렌",
        "시오렌",
        "자비에르",
        "에마레스",
        "드레이크",
        "코비",
        "카스란",
        "조로",
        "엘리나스",
        "타리사",
        "나디아",
        "카르마"
    };
    [SerializeField]
    string[] list_Hero = new string[]
    {
        "리오란",
        "자레스",
        "카스란",
        "에반",
        "타이론",
        "가롤드",
        "칼린",
        "하몬",
        "페이라",
        "아르노",
        "볼드윈",
        "카이론",
        "에드릭",
        "다리우스",
        "하몬",
        "카다스",
        "리오란",
        "페론",
        "그레이든",
        "베른",
        "이안",
        "제로스",
        "페릭스",
        "다스티온",
        "엘라스",
        "알버트",
        "가로드",
        "크리스탄",
        "세르노",
        "카스티엘",
        "다스탄",
        "아일런",
        "볼테론",
        "도란",
        "자이런",
        "에리사",
        "리에나",
        "메릴린",
        "니케아",
        "자레나",
        "리오나",
        "세피라",
        "카산드라",
        "아멜리아",
        "루미아",
        "페라니아",
        "엘리나",
        "노에라",
        "카트린",
        "루미엘",
        "안타드라",
        "셀시우스",
        "자렐라",
        "오렌시아",
        "엘바딘",
        "로자나",
        "브라이스",
        "아이릭",
        "아이젠",
        "가렌",
        "시오렌",
        "자비에르",
        "에마레스",
        "드레이크",
        "코비",
        "카스란",
        "조로",
        "엘리나스",
        "타리사",
        "나디아",
        "카르마"
    };

}
