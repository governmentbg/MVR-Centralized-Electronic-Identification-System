package bg.bulsi.mvr.mpozei.contract.dto.xml;

import bg.bulsi.mvr.common.dto.ExportModel;
import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlElement;
import jakarta.xml.bind.annotation.XmlRootElement;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
@XmlRootElement
@XmlAccessorType(XmlAccessType.FIELD)
public class GuardianDetailsXml implements ExportModel {
	@XmlElement
	private String firstName;
	@XmlElement
	private String secondName;
	@XmlElement
	private String lastName;
	@XmlElement
	private String citizenIdentifierNumber;
	@XmlElement
	private String citizenIdentifierType;
	@XmlElement
	private String citizenship;
	@XmlElement
	private PersonalIdentityDocumentXml personalIdentityDocument;
}
