# Chat
[![codebeat badge](https://codebeat.co/badges/c7d2c716-e0e7-4420-a323-b4f5c5bc4492)](https://codebeat.co/projects/github-com-yri066-chat-main)

Онлайн-чат, в котором Kafka пересылает сообщения между экземплярами приложения и в каждом экземпляре сообщения сохраняются в бд.

Команда для запуска `docker compose up`

Будет запущено 3 экземпляра приложения, Postgresql, Kafka и Redpanda Console UI

Приложения используют следующие адреса:

Клиент 1 [http://localhost:7000](http://localhost:7000)

Клиент 2 [http://localhost:8000](http://localhost:8000)

Клиент 3 [http://localhost:9000](http://localhost:9000)


Веб-интерфейс Redpanda Console [http://localhost:8080](http://localhost:8080/topics)

Имеется возможность взаимодействия через консоль.


