<template>
  <div class="item-warp" v-show="!$parent.loading">
    <div class="item-list">
      <div v-for="(item,index) in items" class="item-item" :class="{right : index != 0 && (index+1) % 3 == 0}">
        <div class="item-header">
          <span class="left" v-show="item.type == 15">Event</span>
          <span class="right">{{item.count}}</span>
        </div>
        <p class="long-name">{{item.name}}</p>
        <p class="detail">{{item.detail}}</p>
      </div>
    </div>
  </div>
</template>
<script>
  export default {
    name: 'items-view',
    data() {
      return {
        items: []
      }
    },
    methods: {
    },
    components: {},
    computed: {
    },
    beforeDestroy() {
    },
    beforeMount() {
      this.$store.dispatch('FETCH_ITEM', this.$route.params.id).then(res => {
        this.$parent.loading = false
        this.items = res.data.data
      })
    }
  }
</script>

<style>
.item-warp{
  color:#6d757a
}
.item-list .left{
    color: red;
    transform: rotate(-60deg);
    position: absolute;
    top: 0px;
    left: -22px;
    font-size: 24px;
}
  .item-list{
    width: 980px;
    margin: 0 auto;
  }
  .item-list .item-item{
    position: relative;
    border-radius: 4px;
    background-color: #fff;
    margin-bottom: 20px;
    padding: 30px;
    text-align: center;
    cursor: pointer;
    width: 30%;
    float: left;
    height: 148px;
    margin-right: 48px;
  }
  .item-list .item-item.right{
      margin-right: 0
  }
  .item-list .item-item:hover{
    color: #00a1d6!important
  }
  .item-list .item-item .item-header{
    height: 20px;
    font-size: 14px
  }
  .item-list .item-item .long-name{
    font-size: 24px;
    text-align: center;
  }
  .item-list .item-item .detail{
      padding-top: 10px;
  }
</style>