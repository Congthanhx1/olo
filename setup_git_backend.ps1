# setup_git_backend.ps1
# Chạy script này trong thư mục DigitalStore để đẩy mã nguồn lên GitHub

$repoUrl = Read-Host "Nhập link GitHub Repository của bạn (ví dụ: https://github.com/YourName/digitalstore-backend.git)"

if (-not $repoUrl) {
    Write-Host "Vui lòng nhập link Repository!" -ForegroundColor Red
    return
}

git init
git add .
git commit -m "Initial commit for deployment"
git branch -M main
git remote add origin $repoUrl
git push -u origin main

Write-Host "Đã đẩy mã nguồn lên GitHub thành công!" -ForegroundColor Green
