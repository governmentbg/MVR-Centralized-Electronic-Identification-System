package bg.bulsi.mvr.common.util;

import com.fasterxml.jackson.core.Version;
import com.fasterxml.jackson.databind.Module;
import com.fasterxml.jackson.databind.module.SimpleDeserializers;
import com.fasterxml.jackson.databind.module.SimpleSerializers;
import org.springframework.data.domain.Sort;

/*
   copy pasted from org.springframework.cloud.openfeign.support.SortJacksonModule
*/
public class SortJacksonModule extends Module {

    @Override
    public String getModuleName() {
        return "SortModule";
    }

    @Override
    public Version version() {
        return new Version(0, 1, 0, "", null, null);
    }

    @Override
    public void setupModule(SetupContext context) {
        SimpleSerializers serializers = new SimpleSerializers();
        serializers.addSerializer(Sort.class, new SortJsonComponent.SortSerializer());
        context.addSerializers(serializers);

        SimpleDeserializers deserializers = new SimpleDeserializers();
        deserializers.addDeserializer(Sort.class, new SortJsonComponent.SortDeserializer());
        context.addDeserializers(deserializers);
    }

}
