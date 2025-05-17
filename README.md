### **Файл README.md**  

#### **На русском:**  

# **MetroCardPass - Система управления проходом через турникеты метро**

![image](https://github.com/user-attachments/assets/51750e18-badc-42fd-88f2-7c47278e9446)

📌 **Описание**  
MetroCardPass - это WPF-приложение для симуляции работы турникетов метро с функцией сбора аналитики. Система предоставляет:  

- Валидацию транспортных карт (формат номера, срок действия)  
- Учет количества поездок (обычные/безлимитные карты)  
- Временную блокировку карт с безлимитными поездками  
- Визуальную индикацию прохода (светофорная система)  
- Статистику использования карт за день  

🛠 **Технологии**  
- Язык программирования: **C#**  
- Платформа: **.NET Framework/WPF**  
- Кэширование: **MemoryCache** (для блокировки карт)  
- Логика валидации: **Регулярные выражения, DateTime**  

⚙️ **Установка**  
1. Клонируйте репозиторий:  
   ```bash  
   git clone https://github.com/D-I-O-N/Metro_turnstile.git 
   ```  
2. Откройте решение в **Visual Studio**.  
3. Запустите проект **CourseOneWpfApp**.  

---

#### **In English:**  

# **MetroCardPass - Metro Turnstile Access Control System**  
📌 **Overview**  
MetroCardPass is a WPF application simulating metro turnstile operations with built-in analytics. Key features:  

- Transport card validation (number format, expiry date)  
- Ride count tracking (regular/unlimited passes)  
- Temporary blocking of unlimited cards  
- Visual pass/deny indicators (traffic light system)  
- Daily usage statistics  

🛠 **Technology Stack**  
- Programming Language: **C#**  
- Platform: **.NET Framework/WPF**  
- Caching: **MemoryCache** (for card blocking)  
- Validation Logic: **Regex, DateTime**  

⚙️ **Installation**  
1. Clone the repository:  
   ```bash  
   git clone https://github.com/D-I-O-N/Metro_turnstile.git 
   ```  
2. Open the solution in **Visual Studio**.  
3. Run the **CourseOneWpfApp** project.  
