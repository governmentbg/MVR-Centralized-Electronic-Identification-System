package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.mpozei.contract.dto.NomenclatureDTO;
import bg.bulsi.mvr.mpozei.contract.dto.NomenclatureTypeDTO;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

@Component
//@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public class NomenclatureMapper {

    public List<NomenclatureTypeDTO> mapToNomenclatures(List<NomenclatureType> nomenclatureTypes, List<ReasonNomenclature> reasonNomenclatures) {
	  if (nomenclatureTypes == null || nomenclatureTypes.isEmpty()) {
	      return Collections.emptyList();
	  }
	   
	  List<NomenclatureTypeDTO> nomenclatures = new ArrayList<>();
	  nomenclatureTypes.forEach(nt -> {
		  NomenclatureTypeDTO nomenclatureTypeDTO = new NomenclatureTypeDTO();
		  nomenclatureTypeDTO.setId(nt.getId());
		  nomenclatureTypeDTO.setName(nt.getName());
		  nomenclatureTypeDTO.setNomenclatures(new ArrayList<>());
		  
		  reasonNomenclatures.stream().filter( r-> r.getNomCode().getId().equals(nt.getId())).forEach( n -> {
			  NomenclatureDTO nomenclatureDTO = new NomenclatureDTO();
			  nomenclatureDTO.setId(n.getId());
			  nomenclatureDTO.setName(n.getName());
			  nomenclatureDTO.setLanguage(n.getLanguage().getValue());
			  nomenclatureDTO.setDescription(n.getDescription());
			  nomenclatureDTO.setPermittedUser(n.getPermittedUser().name());
			  nomenclatureDTO.setTextRequired(n.getTextRequired());
			  nomenclatureTypeDTO.getNomenclatures().add(nomenclatureDTO);
		  });
		  
		  nomenclatures.add(nomenclatureTypeDTO);
	  });
	  
	  return nomenclatures;
	}
}
