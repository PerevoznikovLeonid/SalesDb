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
    seller_id INTEGER NOTа NULL,
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

CREATE TRIGGER trigger_decrease_quantity_after_sale
    AFTER INSERT ON table_sales
    FOR EACH ROW
BEGIN
    UPDATE table_products
    SET quantity = quantity - 1
    WHERE id = NEW.product_id
      AND quantity > 0;

    SELECT RAISE(ABORT, 'Продажа невозможна: недостаточно товара на складе')
    WHERE (SELECT changes() = 0);
END;

CREATE VIEW view_stock AS
SELECT
    id,
    name,
    quantity,
    price
FROM table_products
ORDER BY id;

-- Тестовые данные
INSERT INTO table_persons (id, first_name, last_name, patronymic, address)
VALUES (1, 'Иван', 'Иванов', 'Иванович', 'ул. Ленина, д.1');
INSERT INTO table_phone_numbers (id, person_id, phone_number)
VALUES (1, 1, '+7-900-123-45-67');
UPDATE table_persons SET phone_number_id = 1 WHERE id = 1;

INSERT INTO table_persons (id, first_name, last_name, patronymic, address)
VALUES (2, 'Пётр', 'Петров', 'Петрович', 'ул. Пушкина, д.2');
INSERT INTO table_emails (id, person_id, email)
VALUES (1, 2, 'petrov@example.com');
UPDATE table_persons SET email_id = 1 WHERE id = 2;

INSERT INTO table_products (id, name, quantity, price) VALUES (1, 'Ноутбук', 10, 999.99);
INSERT INTO table_products (id, name, quantity, price) VALUES (2, 'Мышь', 50, 19.99);
INSERT INTO table_products (id, name, quantity, price) VALUES (3, 'Клавиатура', 0, 49.99);