export function formatMoney(v: number, currency = 'CNY') {
  if (Number.isNaN(v)) return '-'
  return new Intl.NumberFormat('zh-CN', {
    style: 'currency',
    currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(v)
}

