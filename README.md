# TextScanner Microservice 
## Рахимзода Фаридун Тоджиддин
## БПИ236

## Описание проекта
Приложение **TextScanner** представляет собой набор микросервисов для пакетного анализа текстовых отчётов студентов и выявления схожести (плагиата). Реализовано на **.NET 8**, контейнеризировано с помощью Docker и управляется через Docker Compose.

### Микросервисы
1. **API Gateway** (Ocelot)  
   - Единая точка входа для клиентов  
   - Маршрутизация запросов к микросервисам хранения и анализа  
   - Поддержка retries/fallback при ошибках  

2. **File Storage Service**  
   - Принимает загрузку файлов `.txt` через HTTP  
   - Присваивает каждому файлу уникальный GUID и сохраняет во volume  
   - Предоставляет API для скачивания по ID  

3. **File Analysis Service**  
   - Считывает сохранённый файл и вычисляет:  
     - Количество абзацев, слов, символов  
     - Частоты использования слов  
     - Jaccard similarity с ранее загруженными отчётами  
   - Сохраняет результаты анализа в **SQLite** через EF Core  
   - Предоставляет endpoints для анализа и получения результатов  


### Структура решения
```text
TextScanner/
├─ ApiGateway/             # Ocelot API Gateway
├─ FileStorageService/     # Сервис хранения .txt
├─ FileAnalysisService/    # Сервис анализа текста
├─ tests/                  # Unit-тесты (xUnit)
├─ docker-compose.yml      # Оркестрация Docker
├─ README.md               # Описание на русском
└─ .gitignore
```

### Запуск проекта
**Требования**
 - Docker Engine ≥ 20.10 + Docker Compose

 - .NET SDK 8.0 (для запуска без Docker и тестов локально)
 
    1. *Через Docker (рекомендуется)*
    ```bash
    docker compose up --build
    ```
 - **Gateway: http://localhost:8000/swagger**
 - **Storage: http://localhost:7001/swagger**
 - **Analysis: http://localhost:7002/swagger**
    
    2. *Локально через .NET CLI*
 ```bash
 cd ApiGateway;       dotnet run
cd FileStorageService; dotnet run
cd FileAnalysisService; dotnet run
```

## Тестирование
```bash
cd tests
dotnet test
```
## Полезные команды
```bash
docker compose down

dotnet clean
```


# Спасибо за внимание :)