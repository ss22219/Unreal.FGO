<template>
    <div class="search-input">
        <input v-model="search" class="bili-input" @click="boxClick" @keydown.enter="searchInput()" @keydown.down="selectDown()" @keydown.up.prevent="selectUp()"
        />
        <div class="search-select" v-show="matchData.length && show">
            <transition-group tag="ul" mode="out-in">
                <li v-for="(value,index) in matchData" :class="{selectback:index==now}" :key="index" @click="searchThis(value)" class="search-select-option search-select-list">
                    {{value}} </li>
            </transition-group>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['data', 'value'],
        beforeMount(){
            this.fire = (t) => {
                if(!t.path.find(p=> p == this.$el))
                    this.show = false
            }
            window.addEventListener('click',this.fire)
        },
        beforeDestroy(){
            window.removeEventListener('click',this.fire)
        },
        watch: {
            // 如果 search 发生改变，这个函数就会运行
            search(newSearch) {
                this.show = true
                this.matchData = this.data.filter(d => newSearch && d.indexOf(newSearch) != -1)
                this.now = 0
                if(this.matchData.length)
                    this.$emit('input', newSearch)
                else if(newSearch && this.data.length)
                    this.search = newSearch.substring(0,newSearch.length - 1)
                if (this.matchData.length == 1 && this.matchData[0] == newSearch)
                    this.matchData = [];
            },value(newValue){
                this.search = newValue
            }
        },
        data: function () {
            return {
                fire : {},
                show : false,
                search: '',
                matchData: [],
                now: -1
            }
        }, methods: {
            boxClick(){
                this.show = true
            },
            //搜索
            searchInput: function () {
                if(!this.matchData.length || this.search == this.matchData[this.now]){
                    this.$emit('enter')
                    this.show = false
                }else
                    this.search = this.matchData[this.now];
            },
            //搜索的内容
            searchThis: function (value) {
                this.search = value;
                this.show = false
            },
            //向下
            selectDown: function () {
                this.now++;
                if (this.now == this.matchData.length) {
                    this.now = 0;
                }
            },
            //向上
            selectUp: function () {
                this.now--;
                if (this.now == -1) {
                    this.now = this.matchData.length - 1;
                }
            }
        }
    }
</script>
<style>
.search-input{float: left; position: relative;}
.selectback{background: #f0f0f0;}
.search-select{
    position: absolute;
    top: 41px;
    z-index: 20;
    background: #fff; 
    width: 550px;
    border: 1px solid #ccc!important;
    _overflow: hidden;
    box-shadow: 1px 1px 3px #ededed;
    -webkit-box-shadow: 1px 1px 3px #ededed;
    -moz-box-shadow: 1px 1px 3px #ededed;
    -o-box-shadow: 1px 1px 3px #ededed;
}
.search-select li{padding: 5px 10px;}
</style>