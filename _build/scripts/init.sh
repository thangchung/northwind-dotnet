
apk add --update curl && rm -rf /var/cache/apk/*

curl -X POST -H "Accept:application/json" -H "Content-Type:application/json" connector:8083/connectors/ -d '
{
  "name": "movie_outbox_source",
  "config": {
    "connector.class": "io.debezium.connector.postgresql.PostgresConnector",
    "tasks.max": "1",

    "database.hostname": "postgres",
    "database.port": "5432",
    "database.user": "movitiz",
    "database.password": "P@ssw0rd",
    "database.dbname": "movitiz",
    "database.server.name": "movitiz",
    "table.whitelist": "movie.movie_outboxes",

    "slot.name": "movie_slot",
    "key.converter": "org.apache.kafka.connect.storage.StringConverter",
    "value.converter": "io.debezium.converters.ByteBufferConverter",
    "value.converter.schemas.registry.url": "http://schema-registry:8081",
    "include.schema.changes": "false",
    "tombstones.on.delete" : "false",
    "internal.key.converter": "org.apache.kafka.connect.json.JsonConverter",
    "internal.value.converter": "org.apache.kafka.connect.json.JsonConverter",

    "transforms": "outbox",
    "transforms.outbox.type": "io.debezium.transforms.outbox.EventRouter",
    "transforms.outbox.table.field.event.id": "id",
    "transforms.outbox.table.field.event.key": "aggregate_id",
    "transforms.outbox.table.field.event.type": "type",
    "transforms.outbox.table.field.event.payload.id": "aggregate_id",
    "transforms.outbox.table.fields.additional.placement": "type:header:eventType",
    "transforms.outbox.route.by.field": "aggregate_type",
    "transforms.outbox.route.topic.replacement": "movie_events"
  }
}'

curl -X POST -H "Accept:application/json" -H "Content-Type:application/json" connector:8083/connectors/ -d '
{
  "name": "movies_table_source",
  "config": {
    "connector.class": "io.debezium.connector.postgresql.PostgresConnector",
    "tasks.max": "1",

    "database.hostname": "postgres",
    "database.port": "5432",
    "database.user": "movitiz",
    "database.password": "P@ssw0rd",
    "database.dbname": "movitiz",
    "database.server.name": "movitiz",
    "table.whitelist": "movie_show.movies"
  }
}'

curl -X POST -H "Accept:application/json" -H "Content-Type:application/json" connector:8083/connectors/ -d '
{
  "name": "movies_es_sink",
  "config": {
    "connector.class": "io.confluent.connect.elasticsearch.ElasticsearchSinkConnector",
    "connection.url": "http://elasticsearch:9200",
    "type.name": "_doc",
    "topics": "movitiz.movie_show.movies",
    "key.ignore": "false",
    "schema.ignore": "true",
    "write.method": "upsert",
    "behavior.on.null.values": "delete",
    "value.converter":"io.confluent.connect.avro.AvroConverter",
    "value.converter.schema.registry.url":"http://schema-registry:8081",
    "key.converter":"io.confluent.connect.avro.AvroConverter",
    "key.converter.schema.registry.url":"http://schema-registry:8081",
    "transforms": "extractKey",
    "transforms.extractKey.type":"org.apache.kafka.connect.transforms.ExtractField$Key",
    "transforms.extractKey.field":"id"
  }
}'
