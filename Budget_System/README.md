# 預算系統 (ASP.NET MVC)

---

## 基本操作流程
  * 使用者登入後畫面顯示申請過的預算資料，隨後在自行操作新增或修改預算，預算細分至各部門科目(例如:業務部有交際費、資訊部有設備維修費等)，之後再針對科目紀錄細項明細(例如:交際費明細，高鐵費、客戶外食費等)，簽核功能依照設定的關卡人員進行簽核。

![image](https://github.com/joycloud/Budget_System/blob/master/Budget_System/pics/01.PNG)

---

## 資料表架構
  * 各個表單的主檔table對應至指定的流程，當此表單確認後開始跑流程，便會以表單代號為主要key產生至流程的table，而流程table為目前表單的關卡位置，此外每次動作也會記錄在歷史的table內並顯示在畫面方便相關使用者查看。

![image](https://github.com/joycloud/Budget_System/blob/master/Budget_System/pics/02.PNG)

---

## 登入畫面
  * 使用者登入。

![image](https://github.com/joycloud/Budget_System/blob/master/Budget_System/pics/03.PNG)

---

## 新增預算
  * 輸入部門代號、月份，系統會判斷當月是否有重複申請。

![image](https://github.com/joycloud/Budget_System/blob/master/Budget_System/pics/04.PNG)

---
