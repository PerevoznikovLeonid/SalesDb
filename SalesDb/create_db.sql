CREATE TABLE table_products(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    quantity INTEGER NOT NULL CHECK ( quantity >= 0 ),
    price REAL NOT NULL
);

CREATE TABLE table_phone_numbers(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    person_id INTEGER NOT NULL,
    phone_number TEXT NOT NULL,
    FOREIGN KEY (person_id) REFERENCES table_persons(id)
                                ON DELETE NO ACTION 
                                ON UPDATE NO ACTION
);

CREATE TABLE table_emails(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    person_id INTEGER NOT NULL,
    email TEXT NOT NULL,
    FOREIGN KEY (person_id) REFERENCES table_persons(id)
                         ON DELETE NO ACTION 
                         ON UPDATE NO ACTION 
);

CREATE TABLE table_persons(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    patronymic TEXT,
    phone_number_id INTEGER,
    email_id INTEGER,
    address TEXT,
    FOREIGN KEY (phone_number_id) REFERENCES table_phone_numbers(id)
                          ON DELETE NO ACTION 
                          ON UPDATE NO ACTION,
    FOREIGN KEY (email_id) REFERENCES table_emails(id)
                          ON DELETE NO ACTION 
                          ON UPDATE NO ACTION
);

CREATE TABLE table_sales(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id INTEGER NOT NULL,
    seller_id INTEGER NOT NULL,
    customer_id INTEGER NOT NULL,
    date TEXT NOT NULL,
    FOREIGN KEY (product_id) REFERENCES table_products(id)
                        ON DELETE NO ACTION 
                        ON UPDATE NO ACTION,
    FOREIGN KEY (seller_id) REFERENCES table_persons(id)
                        ON DELETE NO ACTION 
                        ON UPDATE NO ACTION,
    FOREIGN KEY (customer_id) REFERENCES table_persons(id)
                        ON DELETE NO ACTION
                        ON UPDATE NO ACTION 
);