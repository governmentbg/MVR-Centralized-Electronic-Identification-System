package bg.bulsi.mvr.mpozei.contract.dto.xml;

import jakarta.xml.bind.annotation.XmlAccessType;
import jakarta.xml.bind.annotation.XmlAccessorType;
import jakarta.xml.bind.annotation.XmlElement;
import jakarta.xml.bind.annotation.XmlRootElement;
import jakarta.xml.bind.annotation.adapters.XmlJavaTypeAdapter;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDate;

import bg.bulsi.mvr.common.dto.ExportModel;

@Data
@AllArgsConstructor
@NoArgsConstructor
@XmlRootElement
@XmlAccessorType(XmlAccessType.FIELD)
public class PersonalIdentityDocumentXml implements ExportModel {

	@XmlElement
	private String identityNumber;
	
	@XmlElement
	private String identityType;
	
	@XmlJavaTypeAdapter(LocalDateAdapter.class)
	@XmlElement
	private LocalDate identityIssueDate;
	
	@XmlJavaTypeAdapter(LocalDateAdapter.class)
	@XmlElement
	private LocalDate identityValidityToDate;
}
