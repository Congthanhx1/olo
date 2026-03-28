# setup_and_push.ps1
# Chạy script này để đẩy TOÀN BỘ dự án (Backend + Frontend) lên GitHub 1 lần duy nhất!

Write-Host "--- CHƯƠNG TRÌNH TỰ ĐỘNG DEPLOY DIGITALSTORE ---" -ForegroundColor Cyan

$repoUrl = Read-Host "Dán link GitHub Repository MỚI của bạn vào đây"

if (-not $repoUrl) {
    Write-Host "Lỗi: Bạn chưa nhập link Repo!" -ForegroundColor Red
    pause
    return
}

Write-Host "Đang chuẩn bị mã nguồn..." -ForegroundColor Yellow
git init
git add .
git commit -m "Initial combined deployment with Render Blueprint"
git branch -M main
git remote add origin $repoUrl

Write-Host "Đang đẩy code lên GitHub..." -ForegroundColor Yellow
git push -u origin main

Write-Host ""
Write-Host "CHÚC MỪNG! Code của bạn đã lên GitHub." -ForegroundColor Green
Write-Host "Bây giờ bạn chỉ cần vào Render -> New -> Blueprint -> Chọn Repo này là XONG!" -ForegroundColor White
pause
