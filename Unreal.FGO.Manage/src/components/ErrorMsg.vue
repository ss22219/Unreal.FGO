<template>
    <transition name="fade">
        <div v-show="show" class="toaster-wrp topcenter error" v-bind:style="{ 'margin-left' : width + 'px'}" style="top: 52px;opacity: 1;">
            <span class="error-text msg-text">{{msg}}</span>
        </div>
    </transition>
</template>
<script>
    export default {
        computed: {
            show(){
                return this.$store.state.error.show
            },
            msg(){
                return this.$store.state.error.msg
            },
            width() {
                if (this.$el)
                    return this.$el.offsetWidth / 2
                return 0;
            }
        }
    }
</script>