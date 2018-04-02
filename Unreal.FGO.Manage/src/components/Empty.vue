<template>
    <div id="charge-wrapper">
        <div class="charge-participate fail-stat">
            <h1>无法进行操作...(´；ω；`)</h1>
            <div class="reason">
                <p class="reason-txt">{{msg}}
                </p>
                <img class="" alt="" src="/public/s3.hdslb.com/bfs/static/member/article_empty.png">
            </div>
            <router-link class="part-btn" :to="url">
                {{linkName}} <i class="iconfont icon-arrow1"></i>
            </router-link>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['msg', 'linkName', 'url']
    }
</script>