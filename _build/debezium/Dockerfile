#FROM strimzi/kafka:0.20.0-kafka-2.6.0
FROM debezium/connect:1.7
ENV KAFKA_CONNECT_PLUGIN_PATH=/kafka/connect

# Install Dockerize to be able to eg. wait for Postgres or ElasticSearch startup
USER root
RUN yum -y update && yum -y install wget unzip

#RUN mkdir $KAFKA_CONNECT_PLUGIN_PATH &&\
#    cd $KAFKA_CONNECT_PLUGIN_PATH &&\
#    curl -sfSL https://repo1.maven.org/maven2/io/debezium/debezium-connector-postgres/1.4.2.Final/debezium-connector-postgres-1.4.2.Final-plugin.tar.gz | tar xz

# ADD ./connectors/confluentinc-kafka-connect-jdbc-10.2.1.zip /$KAFKA_CONNECT_PLUGIN_PATH
# RUN unzip $KAFKA_CONNECT_PLUGIN_PATH/confluentinc-kafka-connect-jdbc-10.2.1.zip -d $KAFKA_CONNECT_PLUGIN_PATH

ENV DOCKERIZE_VERSION v0.6.1
RUN wget https://github.com/jwilder/dockerize/releases/download/$DOCKERIZE_VERSION/dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && tar -C /usr/local/bin -xzvf dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && rm dockerize-linux-amd64-$DOCKERIZE_VERSION.tar.gz