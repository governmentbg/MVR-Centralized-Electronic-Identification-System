
package bg.bulsi.mvr.mpozei.backend.soap_impl;

import jakarta.xml.bind.JAXBElement;
import jakarta.xml.bind.annotation.XmlElementDecl;
import jakarta.xml.bind.annotation.XmlRegistry;

import javax.xml.namespace.QName;


/**
 * This object contains factory methods for each 
 * Java content interface and Java element interface 
 * generated in the bg.bulsi.mvr.mpozei.backend.soap package. 
 * <p>An ObjectFactory allows you to programmatically 
 * construct new instances of the Java representation 
 * for XML content. The Java representation of XML 
 * content can consist of schema derived interfaces 
 * and classes representing the binding of schema 
 * type definitions, element declarations and model 
 * groups.  Factory methods for each of these are 
 * provided in this class.
 * 
 */
@XmlRegistry
public class ObjectFactory {

    private static final QName _DataPreparationResult_QNAME = new QName("services.dp.tidis.muehlbauer.de", "dataPreparationResult");

    /**
     * Create a new ObjectFactory that can be used to create new instances of schema derived classes for package: bg.bulsi.mvr.mpozei.backend.soap
     * 
     */
    public ObjectFactory() {
    }

    /**
     * Create an instance of {@link DataPreparationResult }
     * 
     * @return
     *     the new instance of {@link DataPreparationResult }
     */
    public DataPreparationResult createDataPreparationResult() {
        return new DataPreparationResult();
    }

    /**
     * Create an instance of {@link JAXBElement }{@code <}{@link DataPreparationResult }{@code >}
     * 
     * @param value
     *     Java instance representing xml element's value.
     * @return
     *     the new instance of {@link JAXBElement }{@code <}{@link DataPreparationResult }{@code >}
     */
    @XmlElementDecl(namespace = "services.dp.tidis.muehlbauer.de", name = "dataPreparationResult")
    public JAXBElement<DataPreparationResult> createDataPreparationResult(DataPreparationResult value) {
        return new JAXBElement<>(_DataPreparationResult_QNAME, DataPreparationResult.class, null, value);
    }

}
