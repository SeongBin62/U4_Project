[System.Serializable]
public class ItemData 
{
    public int id;              //id
    public string name;         //이름
    public string type;            //구분
    public int typeId;
    public int effect;          //효과 값
    public int usingId;         //사용 조건
    public int monsterId;       //드랍 몬스터
    public string description;  //설명

    public ItemData(int id, string name, string type,int typeId, int effect, int usingId,
                       int monsterId, string description)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.typeId = typeId;
        this.effect = effect;
        this.usingId = usingId;
        this.monsterId = monsterId;
        this.description=description;
    }


}
