CREATE TABLE IF NOT EXISTS koshelek (
                              id serial NOT NULL,
                              serial_number int NOT NULL,
                              text varchar(128) NOT NULL,
                              time_of_sending timestamp without time zone NOT NULL,
                              CONSTRAINT koshelek_pk PRIMARY KEY (id)
                              )