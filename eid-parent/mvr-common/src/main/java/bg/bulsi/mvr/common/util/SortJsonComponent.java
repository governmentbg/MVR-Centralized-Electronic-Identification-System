package bg.bulsi.mvr.common.util;

import bg.bulsi.mvr.common.exception.FaultMVRException;
import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.core.TreeNode;
import com.fasterxml.jackson.databind.*;
import com.fasterxml.jackson.databind.node.ArrayNode;
import org.springframework.data.domain.Sort;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static bg.bulsi.mvr.common.exception.ErrorCode.INTERNAL_SERVER_ERROR;

/*
   copy pasted from org.springframework.cloud.openfeign.support.SortJsonComponent
*/
public class SortJsonComponent {

    public static class SortSerializer extends JsonSerializer<Sort> {

        @Override
        public void serialize(Sort value, JsonGenerator gen, SerializerProvider serializers) throws IOException {
            gen.writeStartArray();
            value.iterator().forEachRemaining(v -> {
                try {
                    gen.writeObject(v);
                }
                catch (IOException e) {
                    throw new FaultMVRException("Couldn't serialize object " + v, INTERNAL_SERVER_ERROR);
                }
            });
            gen.writeEndArray();
        }

        @Override
        public Class<Sort> handledType() {
            return Sort.class;
        }

    }

    public static class SortDeserializer extends JsonDeserializer<Sort> {

        @Override
        public Sort deserialize(JsonParser jsonParser, DeserializationContext deserializationContext)
                throws IOException {
            TreeNode treeNode = jsonParser.getCodec().readTree(jsonParser);
            if (treeNode.isArray()) {
                ArrayNode arrayNode = (ArrayNode) treeNode;
                List<Sort.Order> orders = new ArrayList<>();
                for (JsonNode jsonNode : arrayNode) {
                    Sort.Order order = new Sort.Order(Sort.Direction.valueOf(jsonNode.get("direction").textValue()),
                            jsonNode.get("property").textValue());
                    orders.add(order);
                }
                return Sort.by(orders);
            }
            return null;
        }

        @Override
        public Class<Sort> handledType() {
            return Sort.class;
        }

    }

}
