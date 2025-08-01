namespace eID.PDEAU.API.Responses;

public class FetchNomenclaturesResponseType
{
    private CountryMultilangNomElement[] countryNomElementField;

    private SimpleNomenclature[] simpleNomenclatureField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CountryNomElement")]
    public CountryMultilangNomElement[] CountryNomElement
    {
        get
        {
            return this.countryNomElementField;
        }
        set
        {
            this.countryNomElementField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SimpleNomenclature")]
    public SimpleNomenclature[] SimpleNomenclature
    {
        get
        {
            return this.simpleNomenclatureField;
        }
        set
        {
            this.simpleNomenclatureField = value;
        }
    }
}

public partial class CountryMultilangNomElement : MultilangNomElement
{
    private string iSO2Field;

    private string iSO3Field;

    /// <remarks/>
    public string ISO2
    {
        get
        {
            return this.iSO2Field;
        }
        set
        {
            this.iSO2Field = value;
        }
    }

    /// <remarks/>
    public string ISO3
    {
        get
        {
            return this.iSO3Field;
        }
        set
        {
            this.iSO3Field = value;
        }
    }
}

public partial class MultilangNomElement : NomenclatureEntry
{
    private string nameBGField;

    private string nameENField;

    private int orderingField;

    private bool activeField;

    public MultilangNomElement()
    {
        this.orderingField = 0;
    }

    /// <remarks/>
    public string NameBG
    {
        get
        {
            return this.nameBGField;
        }
        set
        {
            this.nameBGField = value;
        }
    }

    /// <remarks/>
    public string NameEN
    {
        get
        {
            return this.nameENField;
        }
        set
        {
            this.nameENField = value;
        }
    }

    /// <remarks/>
    public int Ordering
    {
        get
        {
            return this.orderingField;
        }
        set
        {
            this.orderingField = value;
        }
    }

    /// <remarks/>
    public bool Active
    {
        get
        {
            return this.activeField;
        }
        set
        {
            this.activeField = value;
        }
    }
}

public partial class NomenclatureEntry
{
    private string codeField;

    /// <remarks/>
    public string Code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }
}

public partial class MetaDefinition
{
    private string codeField;

    private string nameField;

    /// <remarks/>
    public string Code
    {
        get
        {
            return this.codeField;
        }
        set
        {
            this.codeField = value;
        }
    }

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }
}

public partial class SimpleNomenclature
{
    private MetaDefinition definitionField;

    private MultilangNomElement[] nomElementField;

    /// <remarks/>
    public MetaDefinition Definition
    {
        get
        {
            return this.definitionField;
        }
        set
        {
            this.definitionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("NomElement")]
    public MultilangNomElement[] NomElement
    {
        get
        {
            return this.nomElementField;
        }
        set
        {
            this.nomElementField = value;
        }
    }
}
