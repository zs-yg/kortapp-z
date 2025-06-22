// 页面加载完成后执行
document.addEventListener('DOMContentLoaded', function() {
    // 获取当前年份并更新页脚版权信息
    const yearElement = document.querySelector('footer p');
    if (yearElement) {
        const currentYear = new Date().getFullYear();
        yearElement.textContent = yearElement.textContent.replace('2025', currentYear);
    }

    // 为所有导航链接添加点击效果
    const navLinks = document.querySelectorAll('nav a');
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // 可以在这里添加页面切换动画或其他效果
            console.log('导航到: ' + this.getAttribute('href'));
        });
    });

    // 功能列表项悬停效果
    const featureItems = document.querySelectorAll('.features li');
    featureItems.forEach(item => {
        item.addEventListener('mouseover', function() {
            this.style.transform = 'translateX(5px)';
            this.style.transition = 'transform 0.3s ease';
        });
        
        item.addEventListener('mouseout', function() {
            this.style.transform = '';
        });
    });
});
